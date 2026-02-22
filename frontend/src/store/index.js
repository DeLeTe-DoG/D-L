import { createStore } from "vuex";
import axios from "axios";

export default createStore({
    state: {
        lanterns: null,
        drones: null,
    },
    mutations: {
        setLanterns(state, data) {
            state.lanterns = data;
        },
        setDrones(state, data) {
            state.drones = data;
            console.log(state.drones[0])
        },
    },
    actions: {
        getLanterns({ commit }, data) {
            return axios
                .get("https://d-l.onrender.com/api/lanterns")
                .then((response) => {
                    console.log(response);
                    commit("setLanterns", response.data);
                });
        },
        getDrones({ commit }, data) {
            return axios
                .get("https://d-l.onrender.com/api/drones")
                .then((response) => {
                    console.log(response);
                    commit("setDrones", response.data);
                });
        },
        addLantern({ commit, dispatch }, data) {
            axios.post(`https://d-l.onrender.com/api/lanterns/`, {
                lanternName: data.lanternName,
                coordinates: {
                    lat: data.coordinates.lat,
                    lng: data.coordinates.lng,
                },
            })
            .then(response => {
                console.log(response)
                dispatch('getLanterns')
                window.location.reload()
            })
        },
        editLantern({ commit, dispatch }, data) {
            axios.patch(`https://d-l.onrender.com/api/lanterns/${data.id}`, {
                id: data.id,
                lanternName: data.lanternName,
                coordinates: {
                    lat: data.coordinates.lat,
                    lng: data.coordinates.lng,
                },
                status: data.status,
            })
            .then(response => {
                console.log(response)
                dispatch('getLanterns')
                window.location.reload()
            })
        },
        deleteLantern({ commit, dispatch }, id) {
            axios
                .delete(`https://d-l.onrender.com/api/lanterns/${id}`)
                .then((response) => {
                    console.log(response);
                    dispatch("getLanterns");
                    window.location.reload();
                });
        },
        addDrone({ commit, dispatch }, data) {
            axios
                .post(`https://d-l.onrender.com/api/drones/`, {
                    DroneName: data.droneName,
                })
                .then((response) => {
                    console.log(response);
                    dispatch("getDrones");
                    window.location.reload();
                });
        },
        deleteDrone({ commit, dispatch }, id) {
            axios
                .delete(`https://d-l.onrender.com/api/drones/${id}`)
                .then((response) => {
                    console.log(response);
                    dispatch("getDrones");
                    window.location.reload();
                });
        },
    },
});
