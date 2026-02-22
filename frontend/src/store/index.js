import { createStore } from "vuex";
import axios from "axios";

export default createStore({
    state: {
        lanterns: null,
    },
    mutations: {
        setLanterns(state, data) {
            state.lanterns = data;
        },
    },
    actions: {
        getLanterns({ commit }, data) {
            return axios
                .get("http://192.168.4.2:5003/api/lanterns")
                .then((response) => {
                    console.log(response);
                    commit("setLanterns", response.data);
                });
        },
        addItem({ commit, dispatch }, data) {
            axios.post(`http://192.168.4.2:5003/api/lanterns/`, {
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
        editItem({ commit, dispatch }, data) {
            axios.patch(`http://192.168.4.2:5003/api/lanterns/${data.id}`, {
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
        deleteItem({ commit, dispatch }, id) {
            axios.delete(`https://d-l.onrender.com/api/lanterns/${id}`)
            .then(response => {
                console.log(response)
                dispatch('getLanterns')
                window.location.reload()
            })
        },
    },
});
