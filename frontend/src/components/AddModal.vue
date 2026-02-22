<template>
    <div class="modal-wrapper" v-if="showModal" @click.self="closeModal()">
        <div class="modal">
            <div class="page-row">
                <h2 class="page-topic">
                    {{
                        modalType == "add"
                            ? "Добавление"
                            : modalType == "edit"
                              ? "Изменение"
                              : "Удаление"
                    }}
                    {{ itemType == "lantern" ? "столба" : "дрона" }}
                </h2>
                <button class="close-btn" @click="closeModal()">
                    <svg
                        width="12"
                        height="12"
                        viewBox="0 0 12 12"
                        fill="none"
                        xmlns="http://www.w3.org/2000/svg"
                    >
                        <path
                            d="M1 1L11 11M1 11L11 1"
                            stroke="black"
                            stroke-width="2"
                            stroke-linecap="round"
                            stroke-linejoin="round"
                        />
                    </svg>
                </button>
            </div>
            <div class="switcher" v-if="modalType == 'add'">
                <button
                    v-for="item in itemTypes"
                    class="switcher-btn"
                    :class="item.type == itemType ? 'active' : ''"
                    @click="itemType = item.type"
                >
                    {{ item.value }}
                </button>
            </div>
            <form class="modal-form">
                <!-- имя и коорды -->
                <div class="input-group" v-if="modalType != 'delete'">
                    <label for="itemName"
                        >Имя
                        {{ itemType == "lantern" ? "Столба" : "Дрона" }}</label
                    >
                    <input
                        type="text"
                        :placeholder="`Имя ${itemType == 'lantern' ? 'Столба' : 'Дрона'}`"
                        v-model="itemName"
                    />
                </div>
                <p v-else class="confirm-par">Подтвердите удаление</p>
                <div
                    class="input-group"
                    v-if="itemType == 'lantern' && modalType != 'delete'"
                >
                    <label for="itemName">Координаты Столба</label>
                    <div class="coords-inputs">
                        <input
                            type="text"
                            placeholder="Координата X"
                            v-model="itemCoords.lat"
                        />
                        <input
                            type="text"
                            placeholder="Координата Y"
                            v-model="itemCoords.lng"
                        />
                    </div>
                </div>
                <div class="input-group" v-if="modalType != 'delete'">
                    <label for="itemName"
                        >Статус
                        {{ itemType == "lantern" ? "Столба" : "Дрона" }}</label
                    >
                    <select name="itemStatus" id="itemStatus" v-model="itemStatus" v-if="modalType == 'edit'">
                        <option value="1">active</option>
                        <option value="0">inactive</option>
                    </select>
                </div>
                <button
                    type="button"
                    class="confirm"
                    @click="
                        modalType == 'edit'
                            ? handleEdit()
                            : modalType == 'add'
                              ? handleAdding()
                              : handleDelete()
                    "
                >
                    {{
                        modalType == "add"
                            ? "Создать"
                            : modalType == "edit"
                              ? "Изменить"
                              : "Удалить"
                    }}
                </button>
            </form>
        </div>
    </div>
</template>

<script>
import { mapActions } from "vuex";

export default {
    data() {
        return {
            itemType: "lantern",
            itemTypes: [
                {
                    value: "Столб",
                    type: "lantern",
                },
                {
                    value: "Дрон",
                    type: "drone",
                },
            ],
            showModal: false,
            itemCoords: {
                lat: null,
                lng: null,
            },
            itemName: "",
            itemId: null,
            modalType: "add",
            itemStatus: 0,
        };
    },
    methods: {
        ...mapActions({
            editItem: "editItem",
            addItem: "addItem",
            deleteItem: "deleteItem",
        }),
        openModal(data) {
            this.showModal = true;
            if (data) {
                this.itemName = data[0].lanternName;
                this.itemCoords = data[0].coordinates;
                this.modalType = data[1];
                this.itemId = data[0].id;
            }
        },
        closeModal() {
            this.itemName = "";
            this.itemCoords = { lat: null, lng: null };
            this.modalType = "add";
            this.showModal = false;
            this.itemType = 'lantern'
        },
        handleAdding() {
            const sendData = {
                coordinates: this.itemCoords,
                lanternName: this.itemName,
                status: this.itemStatus
            };
            this.addItem(sendData);
        },
        handleEdit() {
            const sendData = {
                id: this.itemId,
                coordinates: this.itemCoords,
                lanternName: this.itemName,
                status: this.itemStatus
            };
            this.editItem(sendData);
        },
        handleDelete() {
            const sendId = this.itemId;
            this.deleteItem(sendId);
        },
    },
};
</script>

<style lang="scss">
.modal-wrapper {
    width: 100vw;
    height: 100vh;
    position: fixed;
    top: 0;
    left: 0;
    background: rgb(0, 0, 0, 0.5);
    display: flex;
    align-items: center;
    justify-content: center;
}
.modal {
    width: 500px;
    // height: 300px;
    border-radius: 15px;
    background: #fff;
    padding: 20px 20px 40px 20px;
    .page-topic {
        margin-bottom: 20px;
    }
    .switcher {
        display: flex;
        flex-direction: row;
        background: var(--color-border);
        border-radius: 10px;
        padding: 3px;
        margin: 15px 0 20px 0;
        &-btn {
            width: 100%;
            height: 40px;
            border: none;
            background: transparent;
            border-radius: 8px;
            &.active {
                background: var(--color-blue);
                color: #fff;
            }
        }
    }
    &-form {
        display: flex;
        flex-direction: column;
        gap: 25px;
        .coords-inputs {
            display: flex;
            flex-direction: row;
            align-items: center;
            gap: 10px;
        }
    }
    .confirm {
        height: 50px;
        background: var(--color-blue);
        color: #fff;
        border: none;
        outline: none;
        border-radius: 15px;
    }
    .confirm-par{
        color: var(--color-text-grey);
        margin-top: 20px;
    }
    .close-btn {
        background: transparent;
        border: none;
        outline: none;
    }
}
</style>
