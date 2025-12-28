<script setup lang="ts">
import { computed, ref } from 'vue'
import Toolbar from 'primevue/toolbar'
import PanelMenu from 'primevue/panelmenu'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import Avatar from 'primevue/avatar'
import Badge from 'primevue/badge'
import Tag from 'primevue/tag'
import Chip from 'primevue/chip'
import RuleManagerPage from './pages/RuleManagerPage.vue'
import CampaignManagerPage from './pages/CampaignManagerPage.vue'

const activePage = ref<'rules' | 'campaigns'>('rules')

const menuItems = [
  {
    label: 'Rule Management',
    icon: 'pi pi-sitemap',
    command: () => {
      activePage.value = 'rules'
    }
  },
  {
    label: 'Campaign Management',
    icon: 'pi pi-megaphone',
    command: () => {
      activePage.value = 'campaigns'
    }
  }
]

const pageTitle = computed(() =>
  activePage.value === 'rules' ? 'Rule Studio' : 'Campaign Studio'
)

const pageSubtitle = computed(() =>
  activePage.value === 'rules'
    ? 'Build, validate, and version your rules.'
    : 'Plan, launch, and monitor campaigns.'
)
</script>

<template>
  <div class="app-shell">
    <aside class="app-sidebar">
      <div class="brand">
        <div class="brand-mark">PV</div>
        <div>
          <div class="brand-title">RuleManager</div>
          <div class="brand-sub">Rule & Campaign Studio</div>
        </div>
      </div>

      <PanelMenu :model="menuItems" class="app-menu" />

      <div class="sidebar-footer">
        <div class="sidebar-tag">
          <Tag icon="pi pi-sparkles" value="Aura Light Noir" />
          <Badge value="LIVE" severity="success" />
        </div>
        <div class="sidebar-note">Connected to Rule Engine API</div>
      </div>
    </aside>

    <div class="app-content">
      <Toolbar class="app-header">
        <template #start>
          <div class="page-title">
            <div class="page-title-text">{{ pageTitle }}</div>
            <div class="page-title-sub">{{ pageSubtitle }}</div>
          </div>
        </template>
        <template #end>
          <span class="p-input-icon-left header-search">
            <i class="pi pi-search" />
            <InputText placeholder="Search in studio" />
          </span>
          <Button icon="pi pi-github" text rounded />
          <Button icon="pi pi-bell" text rounded />
          <Chip label="v4" icon="pi pi-star-fill" class="version-chip" />
          <Avatar label="RM" shape="circle" />
        </template>
      </Toolbar>

      <main class="page-body">
        <RuleManagerPage v-if="activePage === 'rules'" />
        <CampaignManagerPage v-else />
      </main>
    </div>

  </div>
</template>
