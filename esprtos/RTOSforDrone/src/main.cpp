#include <Arduino.h>
#include <WiFi.h>
#include <WebSocketsClient.h>
#include <ArduinoJson.h>

// --- НАСТРОЙКИ СЕТИ ---
const char* ap_ssid     = "Drone_Network";
const char* ap_password = "password123"; 

// --- НАСТРОЙКИ BACKEND ---
const char* server_host = "192.168.4.2"; 
const int   server_port = 5003;
const String droneID    = "DRN_843nkr9p";

WebSocketsClient webSocket;

void processMission(JsonVariant args) {
    digitalWrite(2, HIGH);
    delay(300);
    digitalWrite(2, LOW);
    delay(300);
    digitalWrite(2, HIGH);
    delay(300);
    digitalWrite(2, LOW);
    delay(300);

    JsonArray allVersions = args[0].as<JsonArray>();

    if (allVersions.isNull() || allVersions.size() == 0) {
        Serial.println("   [!] Ошибка: Список маршрутов пуст.");
        return;
    }

    Serial.printf("\n=== ПОЛУЧЕНО ВЕРСИЙ ПУТИ: %d ===\n", allVersions.size());

    for (int i = 0; i < allVersions.size(); i++) {
        JsonArray currentRoute = allVersions[i].as<JsonArray>();
        
        Serial.printf("\nЭТАП МАРШРУТА №%d [%d фонарей]\n", i + 1, currentRoute.size());

        Serial.print(" ПУТЬ: ");
        for (int j = 0; j < currentRoute.size(); j++) {
            Serial.print(currentRoute[j]["id"].as<const char*>());
            if (j < currentRoute.size() - 1) Serial.print(" -> ");
        }
        Serial.println(" -> Конец этапа");

        for (JsonObject lantern : currentRoute) {
            const char* id = lantern["id"];
            float lat = lantern["coordinates"]["lat"];
            float lng = lantern["coordinates"]["lng"];
            int status = lantern["status"];

            Serial.printf("  • %s | %7.4f, %7.4f | Статус: %s\n", 
                          id, lat, lng, 
                          status == 1 ? "![СБОЙ]!" : "ОК");
        }
        Serial.println("------------------------------------------");
    }
}

void handleIncomingMessage(char* json) {
    String input = String(json);

    if (input.endsWith("\x1e")) {
        input.remove(input.length() - 1);
    }

    DynamicJsonDocument doc(4096); 
    DeserializationError error = deserializeJson(doc, input);

    if (error) {

        return; 
    }

    const char* target = doc["target"]; 
    if (target == nullptr) return; 

    Serial.printf("\n[SIGNALR] Команда: %s\n", target);

    if (strcmp(target, "RecieveMission") == 0) {

        processMission(doc["arguments"]);
    } 
    else if (strcmp(target, "EmergencyStop") == 0) {
        Serial.println("-> !!! ЭКСТРЕННАЯ ОСТАНОВКА !!!");
        digitalWrite(2, LOW);
    }
    else if (strcmp(target, "SetLedStatus") == 0) {
        bool state = doc["arguments"][0][0];
        digitalWrite(2, state ? HIGH : LOW);
        Serial.printf("-> LED: %s\n", state ? "ON" : "OFF");
    }
}

void webSocketEvent(WStype_t type, uint8_t * payload, size_t length) {
    switch(type) {
        case WStype_DISCONNECTED:
            Serial.println("[WS] Отключено от сервера");
            break;
        case WStype_CONNECTED:
            Serial.println("[WS] Подключено к SignalR!");
            // Хендшейк SignalR (JSON + 0x1e)
            webSocket.sendTXT("{\"protocol\":\"json\",\"version\":1}\x1e");
            break;
        case WStype_TEXT:
            handleIncomingMessage((char*)payload);

            if (strstr((char*)payload, "LED_ON")) {
                digitalWrite(2, HIGH);
                Serial.println("Светодиод Включён");
            } else if (strstr((char*)payload, "LED_OFF")) {
                digitalWrite(2, LOW);
                Serial.println("Светодиод Выключен");
            }
            break;
    }
}

void setup() {
    Serial.begin(115200);
    pinMode(2, OUTPUT);
    delay(1000);

    // 1. Создаем точку доступа
    WiFi.softAP(ap_ssid, ap_password);
    
    Serial.println("\n--- ТОЧКА ДОСТУПА ЗАПУЩЕНА ---");
    Serial.print("SSID: "); Serial.println(ap_ssid);
    Serial.print("IP ESP32: "); Serial.println(WiFi.softAPIP()); // 192.168.4.1
    Serial.println("------------------------------");

    webSocket.begin(server_host, server_port, "/droneHub?droneId=" + droneID);
    webSocket.onEvent(webSocketEvent);
    webSocket.setReconnectInterval(5000);
}

void loop() {
    webSocket.loop();

    // Каждые 10 секунд шлем тестовое сообщение, чтобы SignalR видел активность
    static unsigned long lastMsg = 0;
    if (millis() - lastMsg > 10000) {
        lastMsg = millis();
        if (webSocket.isConnected()) {
            String msg = "{\"type\":1,\"target\":\"SendAction\",\"arguments\":[\"Heartbeat from ESP32\"]}\x1e";
            webSocket.sendTXT(msg);
            Serial.println("[WS] Сигнал жизни отправлен");
        }
    }
}