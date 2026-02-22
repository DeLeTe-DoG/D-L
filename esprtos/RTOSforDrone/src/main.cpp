// #include <Arduino.h>
// #include <vector>

// // Структура узла дерева
// struct LightNode {
//     uint16_t id;          // ID фонаря
//     uint16_t parentId;    // ID родителя (0 - Дрон-станция)
//     String description;   // Описание для консоли
//     uint8_t status; // 1 - OK, 0 - ERR
// };

// std::vector<LightNode> missionTree;

// void printTree(uint16_t currentParentId, int level) {
//     for (const auto& node : missionTree) {
//         if (node.parentId == currentParentId) {
//             String indent = "";
//             for (int i = 0; i < level; i++) indent += "    ";

//             if (level == 0) {
//                 Serial.println("BASE [Root]");
//             } else {
//                 Serial.print(indent);
//                 Serial.print("└── ");
//                 Serial.print("ID:");
//                 Serial.print(node.id);
//                 Serial.print(" (");
//                 Serial.print(node.description);
//                 Serial.println(")");
//             }

//             printTree(node.id, level + 1);
//         }
//     }
// }

// void printPathToBase(uint16_t targetID) {
//   uint16_t currID = targetID;
//   String path = "";

//   while (currID != 0) {
//     for (auto& node : missionTree) {
//       if (node.id == currID) {
//         path = "ID: " + String(node.id) + (path == "" ? "" : " --> ");
//         currID = node.parentId;
//         break;
//       }
//     }
//   }
//   Serial.println("МАРШРУТ: База -> " + path);
// };

// void scanForErrors() {
//     Serial.println("\n[СИСТЕМА] Начало полного сканирования...");
//     int errorCount = 0;
//     int totalChecked = 0;

//     for (const auto& node : missionTree) {
//         totalChecked++;
//         if (node.status == 0) { 
//             errorCount++;
//             Serial.print("[!] ");
//             Serial.print(node.description);
//             Serial.print(" (ID: ");
//             Serial.print(node.id);
//             Serial.print(") " );
            
//             printPathToBase(node.id); // Строим путь
//         }
//     }

//     Serial.println("--- Сводка сканирования ---");
//     Serial.printf("Проверено узлов: %d\n", totalChecked);
//     Serial.printf("Найдено неисправностей: %d\n", errorCount);
//     Serial.println("---------------------------");
// }

// void setup() {
//     Serial.begin(115200);
//     delay(1000);
//     Serial.println("\n--- ИНИЦИАЛИЗАЦИЯ ДЕРЕВА МИССИИ ---");

//     // Корень (База)
//     missionTree.push_back({1, 0, "Дрон-станция", 1}); 
    
//     // Первая ветка (Север)
//     missionTree.push_back({10, 1, "Ул. Пушкина, столб 1", 1});
//     missionTree.push_back({11, 10, "Ул. Пушкина, столб 2", 1});
//     missionTree.push_back({12, 11, "Ул. Пушкина, столб 3 (Цель)", 0});

//     // Вторая ветка (Запад)
//     missionTree.push_back({20, 1, "Парк, вход", 1});
//     missionTree.push_back({21, 20, "Парк, аллея 1", 0});

//     Serial.println("Карта объектов в памяти дрона:");
//     printTree(0, 0); 
//     Serial.println("-----------------------------------");

//     scanForErrors();
// }

// void loop() {
//   
// }

#include <Arduino.h>
#include <WiFi.h>
#include <WebSocketsClient.h>

// --- НАСТРОЙКИ СЕТИ ---
const char* ap_ssid     = "Drone_Network";
const char* ap_password = "password123"; 

// --- НАСТРОЙКИ BACKEND ---
const char* server_host = "192.168.4.2"; 
const int   server_port = 5003;
const String droneID    = "DRN_843nkr9p";

WebSocketsClient webSocket;

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
            Serial.printf("[WS] Получено от сервера: %s\n", (char*)payload);

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