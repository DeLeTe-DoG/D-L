import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import './assets/styles.scss'
import store from './store'

import { createYmaps } from 'vue-yandex-maps'

const app = createApp(App)

app.use(router)
app.use(createYmaps({
    apikey: 'b20e5499-3072-48d0-98f6-628ca6e4edfc',
}))
app.use(store)

app.mount('#app')