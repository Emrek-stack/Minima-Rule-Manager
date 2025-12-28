import './assets/main.css'
import 'primeicons/primeicons.css'
import 'primevue/resources/primevue.min.css'
import 'primevue/resources/themes/aura-light-noir/theme.css'

import { createApp } from 'vue'
import PrimeVue from 'primevue/config'
import App from './App.vue'

const app = createApp(App)

app.use(PrimeVue)

app.mount('#app')
