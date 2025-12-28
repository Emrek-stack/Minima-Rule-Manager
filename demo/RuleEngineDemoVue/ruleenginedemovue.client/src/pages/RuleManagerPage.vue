<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import Toolbar from 'primevue/toolbar'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Button from 'primevue/button'
import Dialog from 'primevue/dialog'
import InputText from 'primevue/inputtext'
import Textarea from 'primevue/textarea'
import Dropdown from 'primevue/dropdown'
import Chips from 'primevue/chips'
import Tag from 'primevue/tag'
import Message from 'primevue/message'
import TabView from 'primevue/tabview'
import TabPanel from 'primevue/tabpanel'

type RuleEntity = {
  id: string
  name: string
  description: string
  status: number
  createdAt: string
  updatedAt: string
  tags: string[]
}

type RuleVersion = {
  id: string
  ruleId: string
  version: number
  predicateExpression: string
  resultExpression: string
  language: string
  metadata: string | null
  createdAt: string
  isActive: boolean
}

type RuleParameter = {
  id: string
  ruleId: string
  name: string
  type: string
  value: string | null
}

type RuleSyntaxError = {
  chracterAt: number
  line: number
  title: string
  description: string
  helpLink: string
  category: string
}

const rules = ref<RuleEntity[]>([])
const loading = ref(false)
const editorVisible = ref(false)
const validateVisible = ref(false)
const detailVisible = ref(false)
const isEdit = ref(false)
const form = ref<RuleEntity>(createRuleDraft())
const selectedRule = ref<RuleEntity | null>(null)
const validationInput = ref('')
const validationErrors = ref<RuleSyntaxError[]>([])
const versions = ref<RuleVersion[]>([])
const parameters = ref<RuleParameter[]>([])
const notice = ref<{ severity: 'success' | 'error' | 'info'; text: string } | null>(null)

const statusOptions = [
  { label: 'Draft', value: 0 },
  { label: 'Active', value: 1 },
  { label: 'Disabled', value: 2 }
]

const statusLookup = computed(() => new Map(statusOptions.map((item) => [item.value, item.label])))

onMounted(() => {
  void loadRules()
})

function createRuleDraft(): RuleEntity {
  const now = new Date().toISOString()
  return {
    id: '',
    name: '',
    description: '',
    status: 0,
    createdAt: now,
    updatedAt: now,
    tags: []
  }
}

function formatDate(value: string) {
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return value
  return date.toLocaleString()
}

function showNotice(severity: 'success' | 'error' | 'info', text: string) {
  notice.value = { severity, text }
}

async function loadRules() {
  loading.value = true
  notice.value = null
  try {
    const response = await fetch('/api/Rule')
    if (!response.ok) throw new Error(await response.text())
    rules.value = await response.json()
  } catch (error) {
    showNotice('error', 'Failed to load rules.')
  } finally {
    loading.value = false
  }
}

function openCreate() {
  isEdit.value = false
  form.value = createRuleDraft()
  editorVisible.value = true
}

function openEdit(rule: RuleEntity) {
  isEdit.value = true
  form.value = { ...rule, tags: [...(rule.tags || [])] }
  editorVisible.value = true
}

async function saveRule() {
  notice.value = null
  try {
    const payload = { ...form.value, tags: form.value.tags || [] }
    if (isEdit.value) {
      const response = await fetch(`/api/Rule/${payload.id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
      })
      if (!response.ok) throw new Error(await response.text())
      showNotice('success', 'Rule updated.')
    } else {
      const response = await fetch('/api/Rule', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
      })
      if (!response.ok) throw new Error(await response.text())
      showNotice('success', 'Rule created.')
    }
    editorVisible.value = false
    await loadRules()
  } catch (error) {
    showNotice('error', 'Unable to save the rule.')
  }
}

async function deleteRule(rule: RuleEntity) {
  notice.value = null
  try {
    const response = await fetch(`/api/Rule/${rule.id}`, { method: 'DELETE' })
    if (!response.ok) throw new Error(await response.text())
    showNotice('success', 'Rule deleted.')
    await loadRules()
  } catch (error) {
    showNotice('error', 'Unable to delete the rule.')
  }
}

function openValidation(rule: RuleEntity) {
  selectedRule.value = rule
  validationInput.value = rule.description || 'order.Total > 100'
  validationErrors.value = []
  validateVisible.value = true
}

async function validateRule() {
  notice.value = null
  try {
    const response = await fetch('/api/Rule/validate', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(validationInput.value)
    })
    if (!response.ok) throw new Error(await response.text())
    validationErrors.value = await response.json()
    if (!validationErrors.value.length) {
      showNotice('success', 'Syntax looks clean.')
    }
  } catch (error) {
    showNotice('error', 'Validation failed.')
  }
}

async function openDetails(rule: RuleEntity) {
  selectedRule.value = rule
  detailVisible.value = true
  await Promise.all([loadVersions(rule.id), loadParameters(rule.id)])
}

async function loadVersions(ruleId: string) {
  try {
    const response = await fetch(`/api/Rule/versions/${ruleId}`)
    if (!response.ok) throw new Error(await response.text())
    versions.value = await response.json()
  } catch (error) {
    versions.value = []
  }
}

async function loadParameters(ruleId: string) {
  try {
    const response = await fetch(`/api/Rule/parameters/${ruleId}`)
    if (!response.ok) throw new Error(await response.text())
    parameters.value = await response.json()
  } catch (error) {
    parameters.value = []
  }
}
</script>

<template>
  <section class="page-frame">
    <Toolbar class="page-toolbar">
      <template #start>
        <div class="toolbar-title">
          <span>Rule Management</span>
          <small>CRUD, validation, versions, and parameters</small>
        </div>
      </template>
      <template #end>
        <Button icon="pi pi-refresh" label="Refresh" text @click="loadRules" />
        <Button icon="pi pi-plus" label="New Rule" @click="openCreate" />
      </template>
    </Toolbar>

    <Message v-if="notice" :severity="notice.severity" closable @close="notice = null">
      {{ notice.text }}
    </Message>

    <DataTable :value="rules" :loading="loading" dataKey="id" stripedRows class="app-table">
      <Column field="name" header="Name" />
      <Column field="status" header="Status">
        <template #body="{ data }">
          <Tag :value="statusLookup.get(data.status) || 'Unknown'" />
        </template>
      </Column>
      <Column field="updatedAt" header="Last Update">
        <template #body="{ data }">
          <span>{{ formatDate(data.updatedAt) }}</span>
        </template>
      </Column>
      <Column header="Tags">
        <template #body="{ data }">
          <div class="tag-group">
            <Tag v-for="tag in data.tags" :key="tag" :value="tag" severity="info" />
          </div>
        </template>
      </Column>
      <Column header="Actions" style="width: 18rem">
        <template #body="{ data }">
          <div class="table-actions">
            <Button icon="pi pi-eye" text rounded @click="openDetails(data)" />
            <Button icon="pi pi-check" text rounded @click="openValidation(data)" />
            <Button icon="pi pi-pencil" text rounded @click="openEdit(data)" />
            <Button icon="pi pi-trash" text rounded severity="danger" @click="deleteRule(data)" />
          </div>
        </template>
      </Column>
    </DataTable>
  </section>

  <Dialog v-model:visible="editorVisible" modal header="Rule Editor" class="dialog-wide">
    <div class="form-grid">
      <div class="form-field">
        <label for="rule-id">Rule Id</label>
        <InputText id="rule-id" v-model="form.id" placeholder="rule_discount_001" :disabled="isEdit" />
      </div>
      <div class="form-field">
        <label for="rule-name">Name</label>
        <InputText id="rule-name" v-model="form.name" placeholder="VIP discount rule" />
      </div>
      <div class="form-field full">
        <label for="rule-desc">Description</label>
        <Textarea id="rule-desc" v-model="form.description" rows="3" autoResize />
      </div>
      <div class="form-field">
        <label for="rule-status">Status</label>
        <Dropdown id="rule-status" v-model="form.status" :options="statusOptions" optionLabel="label" optionValue="value" />
      </div>
      <div class="form-field">
        <label for="rule-tags">Tags</label>
        <Chips id="rule-tags" v-model="form.tags" separator="," />
      </div>
    </div>
    <template #footer>
      <Button label="Cancel" text @click="editorVisible = false" />
      <Button label="Save Rule" @click="saveRule" />
    </template>
  </Dialog>

  <Dialog v-model:visible="validateVisible" modal header="Validate Rule Expression" class="dialog-wide">
    <div class="form-field full">
      <label for="rule-validate">Expression</label>
      <Textarea id="rule-validate" v-model="validationInput" rows="4" autoResize />
    </div>
    <div v-if="validationErrors.length" class="validation-list">
      <div v-for="error in validationErrors" :key="`${error.line}-${error.chracterAt}`" class="validation-item">
        <div class="validation-title">{{ error.title }}</div>
        <div class="validation-body">{{ error.description }}</div>
        <div class="validation-meta">Line {{ error.line }} - Char {{ error.chracterAt }}</div>
      </div>
    </div>
    <template #footer>
      <Button label="Close" text @click="validateVisible = false" />
      <Button label="Run Validation" icon="pi pi-bolt" @click="validateRule" />
    </template>
  </Dialog>

  <Dialog v-model:visible="detailVisible" modal header="Rule Details" class="dialog-wide">
    <div v-if="selectedRule" class="detail-header">
      <div>
        <div class="detail-title">{{ selectedRule.name }}</div>
        <div class="detail-sub">{{ selectedRule.id }}</div>
      </div>
      <Tag :value="statusLookup.get(selectedRule.status) || 'Unknown'" />
    </div>

    <TabView>
      <TabPanel header="Versions">
        <DataTable :value="versions" dataKey="id" stripedRows>
          <Column field="version" header="Version" />
          <Column field="language" header="Language" />
          <Column field="predicateExpression" header="Predicate" />
          <Column field="resultExpression" header="Result" />
          <Column field="createdAt" header="Created">
            <template #body="{ data }">
              <span>{{ formatDate(data.createdAt) }}</span>
            </template>
          </Column>
          <Column field="isActive" header="Active">
            <template #body="{ data }">
              <Tag :value="data.isActive ? 'Active' : 'Inactive'" :severity="data.isActive ? 'success' : 'warning'" />
            </template>
          </Column>
        </DataTable>
      </TabPanel>
      <TabPanel header="Parameters">
        <DataTable :value="parameters" dataKey="id" stripedRows>
          <Column field="name" header="Name" />
          <Column field="type" header="Type" />
          <Column field="value" header="Value" />
        </DataTable>
      </TabPanel>
    </TabView>
    <template #footer>
      <Button label="Close" text @click="detailVisible = false" />
    </template>
  </Dialog>
</template>
