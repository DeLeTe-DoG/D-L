#include <Arduino.h>
#include <vector>

// Структура узла дерева
struct LightNode {
    uint16_t id;          // ID фонаря
    uint16_t parentId;    // ID родителя (0 - Дрон-станция)
    String description;   // Описание для консоли
    uint8_t status; // 1 - OK, 0 - ERR
};

std::vector<LightNode> missionTree;

void printTree(uint16_t currentParentId, int level) {
    for (const auto& node : missionTree) {
        if (node.parentId == currentParentId) {
            String indent = "";
            for (int i = 0; i < level; i++) indent += "    ";

            if (level == 0) {
                Serial.println("BASE [Root]");
            } else {
                Serial.print(indent);
                Serial.print("└── ");
                Serial.print("ID:");
                Serial.print(node.id);
                Serial.print(" (");
                Serial.print(node.description);
                Serial.println(")");
            }

            printTree(node.id, level + 1);
        }
    }
}

void printPathToBase(uint16_t targetID) {
  uint16_t currID = targetID;
  String path = "";

  while (currID != 0) {
    for (auto& node : missionTree) {
      if (node.id == currID) {
        path = "ID: " + String(node.id) + (path == "" ? "" : " --> ");
        currID = node.parentId;
        break;
      }
    }
  }
  Serial.println("МАРШРУТ: База -> " + path);
};

void scanForErrors() {
    Serial.println("\n[СИСТЕМА] Начало полного сканирования...");
    int errorCount = 0;
    int totalChecked = 0;

    for (const auto& node : missionTree) {
        totalChecked++;
        if (node.status == 0) { // 1 - это ERR
            errorCount++;
            Serial.print("[!] ");
            Serial.print(node.description);
            Serial.print(" (ID: ");
            Serial.print(node.id);
            Serial.print(") " );
            
            printPathToBase(node.id); // Строим путь
        }
    }

    Serial.println("--- Сводка сканирования ---");
    Serial.printf("Проверено узлов: %d\n", totalChecked);
    Serial.printf("Найдено неисправностей: %d\n", errorCount);
    Serial.println("---------------------------");
}

void setup() {
    Serial.begin(115200);
    delay(1000);
    Serial.println("\n--- ИНИЦИАЛИЗАЦИЯ ДЕРЕВА МИССИИ ---");

    // Корень (База)
    missionTree.push_back({1, 0, "Дрон-станция", 1}); 
    
    // Первая ветка (Север)
    missionTree.push_back({10, 1, "Ул. Пушкина, столб 1", 1});
    missionTree.push_back({11, 10, "Ул. Пушкина, столб 2", 1});
    missionTree.push_back({12, 11, "Ул. Пушкина, столб 3 (Цель)", 0});

    // Вторая ветка (Запад)
    missionTree.push_back({20, 1, "Парк, вход", 1});
    missionTree.push_back({21, 20, "Парк, аллея 1", 0});

    Serial.println("Карта объектов в памяти дрона:");
    printTree(0, 0); // Начинаем с ID 0 (виртуальный корень)
    Serial.println("-----------------------------------");

    scanForErrors();
}

void loop() {
    // В этом примере нам не нужен цикл
}