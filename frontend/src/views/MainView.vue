<template>
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
        <table class="columns-journal" v-if="lanterns">
            <thead>
                <tr>
                    <td>Столб</td>
                    <td>Имя</td>
                    <td>Статус</td>
                    <td>Дата</td>
                    <td>ID Дрона</td>
                    <td>Время</td>
                    <td></td>
                </tr>
            </thead>
            <tbody>
                <tr v-for="lant in lanterns">
                    <td>{{ lant.id }}</td>
                    <td>{{ lant.lanternName }}</td>
                    <td>
                        <div class="td-wrapper">
                            <div
                                class="indicator"
                                :class="lant.status ? 'active' : 'inactive'"
                            ></div>
                            {{ lant.status ? "Работает" : "Не работает" }}
                        </div>
                    </td>
                    <td>09/12/25</td>
                    <td>{{ lant.drone || "-" }}</td>
                    <td>30:00</td>
                    <td>
                        <div class="td-wrapper">
                            <button
                                class="edit-btn"
                                @click="
                                    $refs.addModal.openModal([lant, 'edit'])
                                "
                            >
                                <svg
                                    width="18"
                                    height="18"
                                    viewBox="0 0 18 18"
                                    fill="none"
                                    xmlns="http://www.w3.org/2000/svg"
                                >
                                    <path
                                        d="M2 16H3.425L13.2 6.225L11.775 4.8L2 14.575V16ZM1 18C0.716667 18 0.479333 17.904 0.288 17.712C0.0966668 17.52 0.000666667 17.2827 0 17V14.575C0 14.3083 0.0500001 14.054 0.15 13.812C0.25 13.57 0.391667 13.3577 0.575 13.175L13.2 0.575C13.4 0.391667 13.621 0.25 13.863 0.15C14.105 0.0500001 14.359 0 14.625 0C14.891 0 15.1493 0.0500001 15.4 0.15C15.6507 0.25 15.8673 0.4 16.05 0.6L17.425 2C17.625 2.18333 17.7707 2.4 17.862 2.65C17.9533 2.9 17.9993 3.15 18 3.4C18 3.66667 17.954 3.921 17.862 4.163C17.77 4.405 17.6243 4.62567 17.425 4.825L4.825 17.425C4.64167 17.6083 4.429 17.75 4.187 17.85C3.945 17.95 3.691 18 3.425 18H1ZM12.475 5.525L11.775 4.8L13.2 6.225L12.475 5.525Z"
                                        fill="#969696"
                                    />
                                </svg>
                            </button>
                            <button
                                class="delete-btn"
                                @click="
                                    $refs.addModal.openModal([lant, 'delete'])
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
    </div>
    <h2 class="page-topic map-topic">Метки на карте</h2>
    <div id="map"></div>

    <AddModal ref="addModal" />
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
            },
        };
    },
    computed: {
        ...mapState({
            lanterns: (state) => state.lanterns,
        }),
    },
    methods: {
        ...mapActions({
            getLanterns: "getLanterns",
        }),
        loadYandexMap() {
            if (window.ymaps) {
                this.initMap();
                return;
            }

            const script = document.createElement("script");
            script.src =
                "https://api-maps.yandex.ru/2.1/?apikey=${process.env.VUE_APP_YMAPS_KEY}&lang=ru_RU;";
            script.type = "text/javascript";
            script.onload = () => {
                this.initMap();
            };
            document.head.appendChild(script);
        },

        initMap() {
            window.ymaps.ready(() => {
                const map = new window.ymaps.Map("map", {
                    center: [55.796317, 49.106092],
                    zoom: 11,
                });

                // const placemark = new window.ymaps.Placemark(
                //   [55.796317, 49.106092],
                //   {
                //     balloonContent: "Привет! Это Казань"
                //   }
                // );

                // const points = [
                //     { coords: [55.790846, 49.121748], color: "#ff5834" }, //55.790846, 49.121748
                //     { coords: [55.790607, 49.106988], color: "#12b981" },
                // ];
                this.lanterns.forEach((lant) => {
                    const point = {
                        coords: [lant.coordinates.lat, lant.coordinates.lng],
                        color: lant.status ? '#12b981' : '#ff5834'
                    };
                    const circle = new window.ymaps.Circle(
                        [point.coords, 100],
                        {
                            balloonContent: "hello",
                        },
                        {
                            fillColor: "#fff",
                            strokeColor: point.color,
                            strokeWidth: 5,
                        },
                    );
                    map.geoObjects.add(circle);
                });
            });
        },
    },
    mounted() {
        this.getLanterns().then(() => {
            this.loadYandexMap();
        });
    },
};
</script>

<style lang="scss">
.journal-wrapper {
    height: 500px;
    overflow-y: scroll;
    margin-bottom: 20px;
    border-bottom: 1px solid var(--color-border);
    &::-webkit-scrollbar {
        display: none;
    }
}
.columns-journal {
    width: 100%;
    color: var(--color-text-grey);
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
.map-topic {
    margin: 0 0 15px 50px;
}
#map {
    // margin: 50px;
    width: 100%;
    aspect-ratio: 9/4;
    padding: 0 50px;
}
</style>
