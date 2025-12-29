<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
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
import InputNumber from 'primevue/inputnumber'
import Calendar from 'primevue/calendar'
import ToggleButton from 'primevue/togglebutton'

type RuleContent = {
  predicateExpression: string
  resultExpression: string
  language: string
  metadata?: Record<string, unknown>
}

type RuleDefinition = {
  id: string
  name: string
  version: number
  status: number
  createdAt: string
  updatedAt: string
  tags: string[]
  description: string
  content: RuleContent
  parameters: Record<string, unknown>
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

type ValidationResult = {
  isValid: boolean
  errors: string[]
  warnings: string[]
}

type RuleExecutionResult = {
  success: boolean
  result: unknown
  errorMessage: string | null
  duration: string
  metadata: Record<string, unknown>
}

type RuleExecutionAudit = {
  id: string
  ruleId: string
  ruleVersion: number
  input: string
  output: string
  success: boolean
  errorMessage: string | null
  duration: string
  executedAt: string
}

type RuleMetadataDto = {
  title: string
  description: string
  displayFormat: string
  expressionFormat: string
  isPredicate: boolean
  parameterDefinations: Array<{ title: string; displayFormat: string; type: string; description?: string | null }>
}

type RuleCategoryDto = {
  id: number
  name: string
  ruleNames: string[]
}

type RuleEditorForm = {
  id: string
  name: string
  description: string
  status: number
  tags: string[]
  predicateExpression: string
  resultExpression: string
  language: string
  metadataJson: string
  parametersJson: string
}

type OrderRuleInputForm = {
  totalAmount: number
  customerType: string
  stockQuantity: number
  orderTime: Date | null
  orderCount: number
  productCount: number
  city: string
  category: string
}

type VersionForm = {
  predicateExpression: string
  resultExpression: string
  language: string
  activate: boolean
}

const rules = ref<RuleDefinition[]>([])
const loading = ref(false)
const editorVisible = ref(false)
const validateVisible = ref(false)
const detailVisible = ref(false)
const evaluateVisible = ref(false)
const versionVisible = ref(false)
const isEdit = ref(false)
const form = ref<RuleEditorForm>(createRuleDraft())
const versionForm = ref<VersionForm>(createVersionDraft())
const selectedRule = ref<RuleDefinition | null>(null)
const validationInput = ref<OrderRuleInputForm>(createOrderInput())
const validationResult = ref<ValidationResult | null>(null)
const evaluationInput = ref<OrderRuleInputForm>(createOrderInput())
const evaluationResult = ref<RuleExecutionResult | null>(null)
const versions = ref<RuleVersion[]>([])
const parameters = ref<RuleParameter[]>([])
const auditHistory = ref<RuleExecutionAudit[]>([])
const metadata = ref<Record<string, RuleMetadataDto>>({})
const metadataCategories = ref<RuleCategoryDto[]>([])
const metadataPredicate = ref(true)
const metadataCategoryFilter = ref('')
const notice = ref<{ severity: 'success' | 'error' | 'info'; text: string } | null>(null)

const statusOptions = [
  { label: 'Draft', value: 0 },
  { label: 'Active', value: 1 },
  { label: 'Disabled', value: 2 }
]

const statusLookup = computed(() => new Map(statusOptions.map((item) => [item.value, item.label])))

const metadataRows = computed(() =>
  Object.entries(metadata.value).map(([key, value]) => ({ key, ...value }))
)

onMounted(() => {
  void loadRules()
})

watch([metadataPredicate, metadataCategoryFilter], () => {
  if (detailVisible.value) {
    void loadMetadata()
  }
})

function createRuleDraft(): RuleEditorForm {
  return {
    id: '',
    name: '',
    description: '',
    status: 0,
    tags: [],
    predicateExpression: 'Input.TotalAmount > 100m',
    resultExpression: 'true',
    language: 'csharp',
    metadataJson: '{}',
    parametersJson: '{}'
  }
}

function createVersionDraft(): VersionForm {
  return {
    predicateExpression: 'Input.TotalAmount > 100m',
    resultExpression: 'true',
    language: 'csharp',
    activate: true
  }
}

function createOrderInput(): OrderRuleInputForm {
  return {
    totalAmount: 450,
    customerType: 'VIP',
    stockQuantity: 10,
    orderTime: new Date(),
    orderCount: 1,
    productCount: 2,
    city: 'Istanbul',
    category: 'Electronics'
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

function parseJsonField(value: string, label: string) {
  if (!value.trim()) return {}
  try {
    return JSON.parse(value)
  } catch (error) {
    showNotice('error', `${label} JSON is invalid.`)
    return null
  }
}

function buildOrderInput(formValue: OrderRuleInputForm) {
  return {
    totalAmount: formValue.totalAmount,
    customerType: formValue.customerType,
    stockQuantity: formValue.stockQuantity,
    orderTime: formValue.orderTime?.toISOString(),
    orderCount: formValue.orderCount,
    productCount: formValue.productCount,
    city: formValue.city,
    category: formValue.category
  }
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

function openEdit(rule: RuleDefinition) {
  isEdit.value = true
  form.value = {
    id: rule.id,
    name: rule.name,
    description: rule.description,
    status: rule.status,
    tags: [...(rule.tags || [])],
    predicateExpression: rule.content?.predicateExpression ?? '',
    resultExpression: rule.content?.resultExpression ?? '',
    language: rule.content?.language ?? 'csharp',
    metadataJson: JSON.stringify(rule.content?.metadata ?? {}, null, 2),
    parametersJson: JSON.stringify(rule.parameters ?? {}, null, 2)
  }
  editorVisible.value = true
}

async function saveRule() {
  notice.value = null
  const metadata = parseJsonField(form.value.metadataJson, 'Metadata')
  if (metadata === null) return
  const parameters = parseJsonField(form.value.parametersJson, 'Parameters')
  if (parameters === null) return

  try {
    if (isEdit.value) {
      const payload = {
        name: form.value.name,
        description: form.value.description,
        tags: form.value.tags || [],
        status: form.value.status,
        content: {
          predicateExpression: form.value.predicateExpression,
          resultExpression: form.value.resultExpression,
          language: form.value.language,
          metadata
        },
        parameters
      }
      const response = await fetch(`/api/Rule/${form.value.id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
      })
      if (!response.ok) throw new Error(await response.text())
      showNotice('success', 'Rule updated.')
    } else {
      const payload = {
        name: form.value.name,
        description: form.value.description,
        tags: form.value.tags || [],
        content: {
          predicateExpression: form.value.predicateExpression,
          resultExpression: form.value.resultExpression,
          language: form.value.language,
          metadata
        },
        parameters
      }
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

async function deleteRule(rule: RuleDefinition) {
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

function openValidation(rule: RuleDefinition) {
  selectedRule.value = rule
  validationInput.value = createOrderInput()
  validationResult.value = null
  validateVisible.value = true
}

async function validateRule() {
  if (!selectedRule.value) return
  notice.value = null
  try {
    const response = await fetch('/api/Rule/validate', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        ruleId: selectedRule.value.id,
        input: buildOrderInput(validationInput.value)
      })
    })
    if (!response.ok) throw new Error(await response.text())
    validationResult.value = await response.json()
    if (validationResult.value?.isValid) {
      showNotice('success', 'Validation passed.')
    }
  } catch (error) {
    showNotice('error', 'Validation failed.')
  }
}

function openEvaluate(rule: RuleDefinition) {
  selectedRule.value = rule
  evaluationInput.value = createOrderInput()
  evaluationResult.value = null
  evaluateVisible.value = true
}

async function runEvaluation() {
  if (!selectedRule.value) return
  notice.value = null
  try {
    const response = await fetch(`/api/Rule/evaluate/${selectedRule.value.id}`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(buildOrderInput(evaluationInput.value))
    })
    if (!response.ok) throw new Error(await response.text())
    evaluationResult.value = await response.json()
  } catch (error) {
    showNotice('error', 'Evaluation failed.')
  }
}

async function openDetails(rule: RuleDefinition) {
  selectedRule.value = rule
  detailVisible.value = true
  await Promise.all([loadVersions(rule.id), loadParameters(rule.id), loadAudit(rule.id), loadMetadataCategories(), loadMetadata()])
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

async function loadAudit(ruleId: string) {
  try {
    const response = await fetch(`/api/Rule/audit/${ruleId}?limit=50`)
    if (!response.ok) throw new Error(await response.text())
    auditHistory.value = await response.json()
  } catch (error) {
    auditHistory.value = []
  }
}

async function loadMetadata() {
  try {
    const params = new URLSearchParams()
    params.set('isPredicate', metadataPredicate.value ? 'true' : 'false')
    if (metadataCategoryFilter.value.trim()) {
      params.set('categoryIds', metadataCategoryFilter.value)
    }
    const response = await fetch(`/api/rule-metadata?${params.toString()}`)
    if (!response.ok) throw new Error(await response.text())
    const payload = await response.json()
    metadata.value = payload.metadatas || {}
  } catch (error) {
    metadata.value = {}
  }
}

async function loadMetadataCategories() {
  try {
    const response = await fetch('/api/rule-metadata/categories')
    if (!response.ok) throw new Error(await response.text())
    metadataCategories.value = await response.json()
  } catch (error) {
    metadataCategories.value = []
  }
}

function openCreateVersion() {
  versionForm.value = createVersionDraft()
  versionVisible.value = true
}

async function createVersion() {
  if (!selectedRule.value) return
  notice.value = null
  try {
    const payload = {
      content: {
        predicateExpression: versionForm.value.predicateExpression,
        resultExpression: versionForm.value.resultExpression,
        language: versionForm.value.language,
        metadata: {}
      },
      parameters: {},
      activate: versionForm.value.activate
    }
    const response = await fetch(`/api/Rule/${selectedRule.value.id}/versions`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload)
    })
    if (!response.ok) throw new Error(await response.text())
    versionVisible.value = false
    await loadVersions(selectedRule.value.id)
    showNotice('success', 'Version created.')
  } catch (error) {
    showNotice('error', 'Unable to create version.')
  }
}

async function activateVersion(version: RuleVersion) {
  if (!selectedRule.value) return
  notice.value = null
  try {
    const response = await fetch(`/api/Rule/${selectedRule.value.id}/versions/${version.version}/activate`, {
      method: 'POST'
    })
    if (!response.ok) throw new Error(await response.text())
    await loadVersions(selectedRule.value.id)
    showNotice('success', 'Version activated.')
  } catch (error) {
    showNotice('error', 'Unable to activate version.')
  }
}
</script>

<template>
  <section class="page-frame">
    <Toolbar class="page-toolbar">
      <template #start>
        <div class="toolbar-title">
          <span>Rule Management</span>
          <small>Define rules, validate syntax, and inspect versions</small>
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
      <Column field="version" header="Version" />
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
      <Column header="Actions" style="width: 21rem">
        <template #body="{ data }">
          <div class="table-actions">
            <Button icon="pi pi-eye" text rounded @click="openDetails(data)" />
            <Button icon="pi pi-check" text rounded @click="openValidation(data)" />
            <Button icon="pi pi-play" text rounded @click="openEvaluate(data)" />
            <Button icon="pi pi-pencil" text rounded @click="openEdit(data)" />
            <Button icon="pi pi-trash" text rounded severity="danger" @click="deleteRule(data)" />
          </div>
        </template>
      </Column>
    </DataTable>
  </section>

  <Dialog v-model:visible="editorVisible" modal header="Rule Editor" class="dialog-wide">
    <div class="form-grid">
      <div class="form-field" v-if="isEdit">
        <label for="rule-id">Rule Id</label>
        <InputText id="rule-id" v-model="form.id" disabled />
      </div>
      <div class="form-field">
        <label for="rule-name">Name</label>
        <InputText id="rule-name" v-model="form.name" placeholder="VIP discount rule" />
      </div>
      <div class="form-field full">
        <label for="rule-desc">Description</label>
        <Textarea id="rule-desc" v-model="form.description" rows="2" autoResize />
      </div>
      <div class="form-field">
        <label for="rule-status">Status</label>
        <Dropdown id="rule-status" v-model="form.status" :options="statusOptions" optionLabel="label" optionValue="value" />
      </div>
      <div class="form-field">
        <label for="rule-tags">Tags</label>
        <Chips id="rule-tags" v-model="form.tags" separator="," />
      </div>
      <div class="form-field full">
        <label for="rule-predicate">Predicate Expression</label>
        <Textarea id="rule-predicate" v-model="form.predicateExpression" rows="3" autoResize />
      </div>
      <div class="form-field full">
        <label for="rule-result">Result Expression</label>
        <Textarea id="rule-result" v-model="form.resultExpression" rows="3" autoResize />
      </div>
      <div class="form-field">
        <label for="rule-language">Language</label>
        <InputText id="rule-language" v-model="form.language" />
      </div>
      <div class="form-field full">
        <label for="rule-metadata">Content Metadata (JSON)</label>
        <Textarea id="rule-metadata" v-model="form.metadataJson" rows="3" autoResize />
      </div>
      <div class="form-field full">
        <label for="rule-parameters">Parameters (JSON)</label>
        <Textarea id="rule-parameters" v-model="form.parametersJson" rows="3" autoResize />
      </div>
    </div>
    <template #footer>
      <Button label="Cancel" text @click="editorVisible = false" />
      <Button label="Save Rule" @click="saveRule" />
    </template>
  </Dialog>

  <Dialog v-model:visible="validateVisible" modal header="Validate Rule" class="dialog-wide">
    <div class="form-grid">
      <div class="form-field">
        <label for="val-total">Total Amount</label>
        <InputNumber id="val-total" v-model="validationInput.totalAmount" />
      </div>
      <div class="form-field">
        <label for="val-customer">Customer Type</label>
        <InputText id="val-customer" v-model="validationInput.customerType" />
      </div>
      <div class="form-field">
        <label for="val-stock">Stock Quantity</label>
        <InputNumber id="val-stock" v-model="validationInput.stockQuantity" />
      </div>
      <div class="form-field">
        <label for="val-order-time">Order Time</label>
        <Calendar id="val-order-time" v-model="validationInput.orderTime" showIcon showTime hourFormat="24" />
      </div>
      <div class="form-field">
        <label for="val-order-count">Order Count</label>
        <InputNumber id="val-order-count" v-model="validationInput.orderCount" />
      </div>
      <div class="form-field">
        <label for="val-product-count">Product Count</label>
        <InputNumber id="val-product-count" v-model="validationInput.productCount" />
      </div>
      <div class="form-field">
        <label for="val-city">City</label>
        <InputText id="val-city" v-model="validationInput.city" />
      </div>
      <div class="form-field">
        <label for="val-category">Category</label>
        <InputText id="val-category" v-model="validationInput.category" />
      </div>
    </div>
    <div v-if="validationResult" class="validation-list">
      <div v-if="validationResult.isValid" class="validation-item">
        <div class="validation-title">Validation passed</div>
      </div>
      <div v-else class="validation-item">
        <div class="validation-title">Errors</div>
        <div v-for="error in validationResult.errors" :key="error" class="validation-body">{{ error }}</div>
      </div>
      <div v-if="validationResult.warnings.length" class="validation-item">
        <div class="validation-title">Warnings</div>
        <div v-for="warning in validationResult.warnings" :key="warning" class="validation-body">{{ warning }}</div>
      </div>
    </div>
    <template #footer>
      <Button label="Close" text @click="validateVisible = false" />
      <Button label="Run Validation" icon="pi pi-bolt" @click="validateRule" />
    </template>
  </Dialog>

  <Dialog v-model:visible="evaluateVisible" modal header="Evaluate Rule" class="dialog-wide">
    <div class="form-grid">
      <div class="form-field">
        <label for="eval-total">Total Amount</label>
        <InputNumber id="eval-total" v-model="evaluationInput.totalAmount" />
      </div>
      <div class="form-field">
        <label for="eval-customer">Customer Type</label>
        <InputText id="eval-customer" v-model="evaluationInput.customerType" />
      </div>
      <div class="form-field">
        <label for="eval-stock">Stock Quantity</label>
        <InputNumber id="eval-stock" v-model="evaluationInput.stockQuantity" />
      </div>
      <div class="form-field">
        <label for="eval-order-time">Order Time</label>
        <Calendar id="eval-order-time" v-model="evaluationInput.orderTime" showIcon showTime hourFormat="24" />
      </div>
      <div class="form-field">
        <label for="eval-order-count">Order Count</label>
        <InputNumber id="eval-order-count" v-model="evaluationInput.orderCount" />
      </div>
      <div class="form-field">
        <label for="eval-product-count">Product Count</label>
        <InputNumber id="eval-product-count" v-model="evaluationInput.productCount" />
      </div>
      <div class="form-field">
        <label for="eval-city">City</label>
        <InputText id="eval-city" v-model="evaluationInput.city" />
      </div>
      <div class="form-field">
        <label for="eval-category">Category</label>
        <InputText id="eval-category" v-model="evaluationInput.category" />
      </div>
    </div>
    <div v-if="evaluationResult" class="validation-list">
      <div class="validation-item">
        <div class="validation-title">Execution Result</div>
        <div class="validation-body">Success: {{ evaluationResult.success }}</div>
        <div v-if="evaluationResult.errorMessage" class="validation-body">Error: {{ evaluationResult.errorMessage }}</div>
        <div class="validation-body">Duration: {{ evaluationResult.duration }}</div>
        <div class="validation-body">Result: {{ JSON.stringify(evaluationResult.result, null, 2) }}</div>
      </div>
    </div>
    <template #footer>
      <Button label="Close" text @click="evaluateVisible = false" />
      <Button label="Run Evaluation" icon="pi pi-play" @click="runEvaluation" />
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
        <div class="table-actions">
          <Button icon="pi pi-plus" label="New Version" @click="openCreateVersion" />
        </div>
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
          <Column header="Actions">
            <template #body="{ data }">
              <Button v-if="!data.isActive" icon="pi pi-check-circle" text @click="activateVersion(data)" />
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
      <TabPanel header="Audit">
        <DataTable :value="auditHistory" dataKey="id" stripedRows>
          <Column field="executedAt" header="Executed">
            <template #body="{ data }">
              <span>{{ formatDate(data.executedAt) }}</span>
            </template>
          </Column>
          <Column field="ruleVersion" header="Version" />
          <Column field="success" header="Success">
            <template #body="{ data }">
              <Tag :value="data.success ? 'True' : 'False'" :severity="data.success ? 'success' : 'danger'" />
            </template>
          </Column>
          <Column field="duration" header="Duration" />
          <Column field="errorMessage" header="Error" />
        </DataTable>
      </TabPanel>
      <TabPanel header="Metadata">
        <div class="form-grid">
          <div class="form-field">
            <label for="metadata-scope">Scope</label>
            <ToggleButton id="metadata-scope" v-model="metadataPredicate" onLabel="Predicate" offLabel="Result" />
          </div>
          <div class="form-field full">
            <label for="metadata-category">Category Ids (comma separated)</label>
            <InputText id="metadata-category" v-model="metadataCategoryFilter" placeholder="1,2,3" />
          </div>
        </div>

        <DataTable :value="metadataRows" dataKey="key" stripedRows>
          <Column field="key" header="Rule Name" />
          <Column field="title" header="Title" />
          <Column field="expressionFormat" header="Expression Format" />
          <Column field="description" header="Description" />
        </DataTable>

        <div class="metadata-categories" v-if="metadataCategories.length">
          <div class="validation-title">Categories</div>
          <div v-for="category in metadataCategories" :key="category.id" class="validation-body">
            {{ category.id }} - {{ category.name }} ({{ category.ruleNames.length }} rules)
          </div>
        </div>
      </TabPanel>
    </TabView>
    <template #footer>
      <Button label="Close" text @click="detailVisible = false" />
    </template>
  </Dialog>

  <Dialog v-model:visible="versionVisible" modal header="Create New Version" class="dialog-wide">
    <div class="form-grid">
      <div class="form-field full">
        <label for="version-predicate">Predicate Expression</label>
        <Textarea id="version-predicate" v-model="versionForm.predicateExpression" rows="3" autoResize />
      </div>
      <div class="form-field full">
        <label for="version-result">Result Expression</label>
        <Textarea id="version-result" v-model="versionForm.resultExpression" rows="3" autoResize />
      </div>
      <div class="form-field">
        <label for="version-language">Language</label>
        <InputText id="version-language" v-model="versionForm.language" />
      </div>
      <div class="form-field">
        <label for="version-activate">Activate Now</label>
        <ToggleButton id="version-activate" v-model="versionForm.activate" onLabel="Active" offLabel="Inactive" />
      </div>
    </div>
    <template #footer>
      <Button label="Cancel" text @click="versionVisible = false" />
      <Button label="Create Version" @click="createVersion" />
    </template>
  </Dialog>
</template>
