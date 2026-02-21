import { createRouter, createWebHistory } from "vue-router";

const routes = [
    {
        path: '/',
        name: 'main',
        component: () => import('../views/MainView.vue'),
    },
    {
        path: '/plans',
        name: 'plans',
        component: () => import('../views/PlansView.vue'),
    },
]

const router = createRouter({
    history: createWebHistory(),
    routes,
})

export default router