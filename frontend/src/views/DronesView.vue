<template>
    <!-- <button @click="sendExample()">send</button> -->
    <div class="page-row">
        <h2 class="page-topic">Информация</h2>
        <button class="add-btn" @click="$refs.addModal.openModal()">
            <svg width="20px" height="20px">
                <rect width="20" height="2" x="0" y="9" fill="#418cff" />
                <rect width="2" height="20" x="9" y="0" fill="#418cff" />
            </svg>
        </button>
    </div>
    <div class="journal-wrapper">
        <table class="drones-journal" v-if="drones">
            <thead>
                <tr>
                    <td>ID Дрона</td>
                    <td>Имя</td>
                    <td>Статус</td>
                    <td>Заряд батареи</td>
                    <!-- <td>Время</td> -->
                    <td></td>
                </tr>
            </thead>
            <tbody>
                <tr v-for="drone in drones">
                    <td>{{ drone.droneID }}</td>
                    <td>{{ drone.droneName }}</td>
                    <td>
                        <div class="td-wrapper">
                            <div
                                class="indicator"
                                :class="
                                    drone.droneStatus ? 'active' : 'inactive'
                                "
                            ></div>
                            {{ drone.status ? "Работает" : "Не работает" }}
                        </div>
                    </td>
                    <td>{{ drone.battery }} %</td>
                    <!-- <td>30:00</td> -->
                    <td>
                        <div class="td-wrapper">
                            <button
                                class="delete-btn"
                                @click="
                                    $refs.addModal.openModal([drone, 'delete'])
                                "
                            >
                                <svg
                                    width="18"
                                    height="20"
                                    viewBox="0 0 18 20"
                                    fill="none"
                                    xmlns="http://www.w3.org/2000/svg"
                                >
                                    <path
                                        d="M1 5H17M7 9V15M11 9V15M2 5L3 17C3 17.5304 3.21071 18.0391 3.58579 18.4142C3.96086 18.7893 4.46957 19 5 19H13C13.5304 19 14.0391 18.7893 14.4142 18.4142C14.7893 18.0391 15 17.5304 15 17L16 5M6 5V2C6 1.73478 6.10536 1.48043 6.29289 1.29289C6.48043 1.10536 6.73478 1 7 1H11C11.2652 1 11.5196 1.10536 11.7071 1.29289C11.8946 1.48043 12 1.73478 12 2V5"
                                        stroke="#969696"
                                        stroke-width="2"
                                        stroke-linecap="round"
                                        stroke-linejoin="round"
                                    />
                                </svg>
                            </button>
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>
        <p v-else>Нет данных по дронам</p>
    </div>
    <AddModal ref="addModal" itemType="drone" />
</template>

<script>
import axios from "axios";

import AddModal from "@/components/AddModal.vue";
import { mapActions, mapState } from "vuex";

export default {
    components: { AddModal },
    data() {
        return {
            mapSettings: {
                apiKey: "b20e5499-3072-48d0-98f6-628ca6e4edfc",
                suggestApiKey: "",
                lang: "ru_RU",
                coordorder: "latlong",
                enterprise: false,
                version: "2.1",
                example: "",
            },
        };
    },
    computed: {
        ...mapState({
            drones: (state) => state.drones,
        }),
    },
    methods: {
        ...mapActions({
            getDrones: "getDrones",
        }),
    },
    mounted() {
        this.getDrones();
    },
};
</script>

<style lang="scss">
.journal-wrapper {
    overflow-y: scroll;
    margin-bottom: 20px;
    border-bottom: 1px solid var(--color-border);
    &::-webkit-scrollbar {
        display: none;
    }
}
.drones-journal {
    width: 100%;
    thead {
        color: var(--color-text-grey);
    }
    td {
        padding: 25px 0;
        border-bottom: 1px solid var(--color-border);
        .td-wrapper {
            display: flex;
            flex-direction: row;
            align-items: center;
            gap: 10px;
            button {
                border: none;
                outline: none;
                background: transparent;
                &.edit-btn {
                    margin-left: auto;
                }
            }
        }
    }
    .indicator {
        width: 10px;
        aspect-ratio: 1/1;
        border-radius: 50vi;
        background: var(--color-border);
        &.active {
            background: var(--color-green);
        }
        &.inactive {
            background: var(--color-red);
        }
    }
}
</style>
