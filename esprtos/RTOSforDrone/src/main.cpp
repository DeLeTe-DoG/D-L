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


const char* ssid = "MTSRouter_A773";
const char* password = "57329512";
const char* host = "192.168.1.162";
const int port = 5003;

WebSocketsClient webSocket;

void webSocketEvent(WStype_t type, uint8_t * payload, size_t length) {
    switch(type) {
        case WStype_DISCONNECTED:
            Serial.println("[WS] Статус: Отключено от сервера");
            break;
            
        case WStype_CONNECTED:
            Serial.println("[WS] Статус: Подключено! Отправляю Handshake...");
            // Обязательное сообщение для протокола SignalR (JSON + спецсимвол 0x1e)
            webSocket.sendTXT("{\"protocol\":\"json\",\"version\":1}\x1e");
            break;

        case WStype_TEXT:
            Serial.printf("[WS] Получены данные: %s\n", (char*)payload);
            break;

        case WStype_ERROR:
            Serial.println("[WS] ОШИБКА соединения!");
            break;

        case WStype_PING:
            Serial.println("[WS] Пинг...");
            break;
    }
}

void setup() {
    Serial.begin(115200);
    delay(1000);


    WiFi.begin(ssid, password);
    Serial.print("Подключение к WiFi");
    while (WiFi.status() != WL_CONNECTED) {
        delay(500);
        Serial.print(".");
    }
    Serial.println("\n[WiFi] Подключено! IP ESP32: " + WiFi.localIP().toString());


    webSocket.begin(host, port, "/droneHub");
    webSocket.onEvent(webSocketEvent);
    webSocket.setReconnectInterval(5000);
}

void loop() {
    webSocket.loop();
}