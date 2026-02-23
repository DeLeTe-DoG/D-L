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

void blinkTwice() {
    for (int i = 0; i < 2; i++) {
        digitalWrite(2, HIGH);
        delay(100);
        digitalWrite(2, LOW);
        delay(100);
    }
}

void runFlightStage(int stageNum, String pathDesc) {
    Serial.printf("\n>>> СТАРТ ЭТАПА №%d: %s\n", stageNum, pathDesc.c_str());
    
    // 1. СИГНАЛ ВЗЛЕТА (Два быстрых мигания)
    blinkTwice(); 
    
    // 2. ПРОЦЕСС ПОЛЕТА (Горит постоянно)
    digitalWrite(2, HIGH);
    Serial.println("    [ПОЛЕТ...] Дрон перемещается по координатам");
    
    // Имитируем полет 3 секунды. 
    // Используем цикл с webSocket.loop(), чтобы соединение не разорвалось
    for (int i = 0; i < 30; i++) {
        webSocket.loop(); 
        delay(100); 
    }
    
    // 3. СИГНАЛ ПОСАДКИ
    digitalWrite(2, LOW);
    Serial.println("    [ГОТОВО] Точка достигнута, фиксация.");
    blinkTwice();
    
    delay(1000); // Пауза перед следующим взлетом
}

void sendFixToServer(const char* lanternId) {
    if (webSocket.isConnected()) {
        String msg = "{\"type\":1,\"target\":\"FixLantern\",\"arguments\":[\"" + String(lanternId) + "\"]}\x1e";
        webSocket.sendTXT(msg);
        Serial.printf("   [WS] Отчет об исправлении %s отправлен!\n", lanternId);
    }
}

void sendMissionComplete() {
    if (webSocket.isConnected()) {
        String msg = "{\"type\":1,\"target\":\"CompleteMission\",\"arguments\":[]}\x1e";
        webSocket.sendTXT(msg);
        Serial.println("   [WS] Сигнал завершения миссии отправлен.");
    }
}


void processMission(JsonVariant args) {
    // В SignalR аргументы приходят в массиве. Наш список маршрутов - в args[0]
    JsonArray allVersions = args[0].as<JsonArray>();
    
    if (allVersions.isNull() || allVersions.size() == 0) {
        Serial.println("   [!] Маршрут пуст.");
        return;
    }

    int totalStages = allVersions.size();
    Serial.printf("\n=== ЗАПУСК МИССИИ: %d ЭТАПОВ ===\n", totalStages);

    for (int i = 0; i < totalStages; i++) {
        JsonArray currentRoute = allVersions[i].as<JsonArray>();
        
        // 1. Формируем описание пути для консоли
        String pathDesc = "";
        for (int j = 0; j < currentRoute.size(); j++) {
            pathDesc += currentRoute[j]["id"].as<String>();
            if (j < currentRoute.size() - 1) pathDesc += " -> ";
        }
        
        Serial.printf("\nЭТАП №%d: %s\n", i + 1, pathDesc.c_str());

        // 2. Проходим по точкам этапа
        for (int j = 0; j < currentRoute.size(); j++) {
            JsonObject lantern = currentRoute[j];
            const char* id = lantern["id"] | "??";
            int status = lantern["status"] | 0;

            Serial.printf(" -> Точка %s ", id);

            if (status == 1) {
                Serial.println("[СБОЙ] - Ремонтирую...");
                // Имитация работы
                for(int r = 0; r < 5; r++) {
                    digitalWrite(2, HIGH); delay(200); 
                    digitalWrite(2, LOW);  delay(200);
                    webSocket.loop(); // Поддерживаем связь во время пауз
                }
                sendFixToServer(id);
            } else {
                Serial.println("[OK] - Пролет");
                delay(500);
            }
        }
    }

    Serial.println("\n=== МИССИЯ ЗАВЕРШЕНА ПОЛНОСТЬЮ ===");
    sendMissionComplete(); // Освобождаем дрон на сервере
}

void flight(JsonVariant args) {
    JsonArray allVersions = args[0].as<JsonArray>();
    if (allVersions.isNull() || allVersions.size() == 0) return;

    int totalStages = allVersions.size() + 1;
    Serial.printf("\n=== ПОЛУЧЕНО ЗАДАНИЕ: %d ЭТАПОВ ===\n", totalStages);

    for (int i = 0; i < totalStages; i++) {
        JsonArray currentRoute = allVersions[i].as<JsonArray>();
        
        // Собираем описание пути для консоли (L1 -> L2...)
        String pathDesc = "";
        for (int j = 0; j < currentRoute.size(); j++) {
            pathDesc += currentRoute[j]["id"].as<String>();
            if (j < currentRoute.size() - 1) pathDesc += " -> ";

            const char* id = currentRoute[j]["id"];
            int status = currentRoute[j]["status"];

            if (status == 1) {
                Serial.printf("  [!] Фонарь %s СЛОМАН. Начинаю ремонт...\n", id);

                // Визуальная индикация ремонта (например, быстрое мигание)
                for(int r = 0; r < 5; r++) {
                    digitalWrite(2, HIGH); 
                    delay(500); 
                    digitalWrite(2, LOW); 
                    delay(500);
                }

                // ОТПРАВКА ДАННЫХ НА СЕРВЕР
                sendFixToServer(id);

                Serial.println("  [OK] Фонарь исправлен.");
            }

        }

        // Запускаем симуляцию для этого этапа
        runFlightStage(i + 1, pathDesc);
    }

    Serial.println("\n=== МИССИЯ ЗАВЕРШЕНА ПОЛНОСТЬЮ ===");
}

void handleIncomingMessage(char* json) {
    String input = String(json);
    if (input.endsWith("\x1e")) input.remove(input.length() - 1);

    // Увеличили буфер для больших деревьев
    DynamicJsonDocument doc(8192); 
    DeserializationError error = deserializeJson(doc, input);
    
    if (error) return; 

    const char* target = doc["target"]; 
    if (target == nullptr) return; 

    if (strcmp(target, "RecieveMission") == 0) {
        // Вызываем ОДНУ функцию, которая делает и полет, и ремонт
        processMission(doc["arguments"]);
    } 
    else if (strcmp(target, "EmergencyStop") == 0) {
        Serial.println("!!! ЭКСТРЕННАЯ ОСТАНОВКА !!!");
        digitalWrite(2, LOW);
        // Здесь можно добавить сброс флага занятости, если нужно
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