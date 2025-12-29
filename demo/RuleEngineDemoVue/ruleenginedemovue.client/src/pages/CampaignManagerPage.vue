<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import Toolbar from 'primevue/toolbar'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Button from 'primevue/button'
import Dialog from 'primevue/dialog'
import InputText from 'primevue/inputtext'
import Textarea from 'primevue/textarea'
import InputNumber from 'primevue/inputnumber'
import Calendar from 'primevue/calendar'
import ToggleButton from 'primevue/togglebutton'
import Message from 'primevue/message'
import Tag from 'primevue/tag'
import Divider from 'primevue/divider'
import Dropdown from 'primevue/dropdown'

type Campaign = {
  id: number
  code: string
  name: string
  modulId: number | null
  startDate: string
  endDate: string
  priority: number
  predicate: string
  result: string
  description: string | null
  quota: number | null
  promotionCode: string | null
  createDate: string
  campaignTypes: number | null
  usage: string | null
  couponCodeId: number | null
  departmentId: number | null
  cancelReasonId: number | null
  cancelSourceId: number | null
}

type CampaignForm = {
  id: number
  code: string
  name: string
  modulId: number | null
  startDate: Date | null
  endDate: Date | null
  priority: number
  predicate: string
  result: string
  description: string
  quota: number | null
  promotionCode: string
  createDate: Date | null
  campaignTypes: number | null
  usage: string
  couponCodeId: number | null
  departmentId: number | null
  cancelReasonId: number | null
  cancelSourceId: number | null
}

type CampaignRuleInputForm = {
  totalAmount: number
  customerType: string
  orderCount: number
  productCount: number
  orderTime: Date | null
  city: string
  category: string
  usageCount: number
}

type Price = {
  value: number
  currency: string
}

type DemoTravelProductForm = {
  key: string
  totalPrice: Price
}

type CampaignOutput = {
  totalDiscount: Price
  campaignProductDiscount: null | {
    productKey: string
    discountAmount: Price
    discountPercentage: number
    discountType: string
  }
}

type AvailableCampaign = {
  campaignCode: string
  campaignName: string
  campaignType: number | null
  productKey: string
  targetResults: Array<{ productKey: string; discount: Price }>
}

const campaigns = ref<Campaign[]>([])
const loading = ref(false)
const editorVisible = ref(false)
const isEdit = ref(false)
const form = ref<CampaignForm>(createCampaignDraft())
const notice = ref<{ severity: 'success' | 'error' | 'info'; text: string } | null>(null)

const filters = reactive({
  after: null as Date | null,
  moduleId: 1 as number | null
})

const checkItems = ref([{ key: 'isVip', enabled: true }])
const checkResults = ref<Record<string, boolean> | null>(null)
const checkLoading = ref(false)

const quotaDialogVisible = ref(false)
const quotaValue = ref(0)
const quotaResult = ref<boolean | null>(null)
const quotaTarget = ref<Campaign | null>(null)

const evaluationInput = ref<CampaignRuleInputForm>(createCampaignInput())
const evaluationResults = ref<CampaignOutput[]>([])
const evaluationLoading = ref(false)

const availableInput = ref<CampaignRuleInputForm>(createCampaignInput())
const availableProducts = ref<DemoTravelProductForm[]>([
  { key: 'FLIGHT-100', totalPrice: { value: 1200, currency: 'TRY' } },
  { key: 'HOTEL-200', totalPrice: { value: 2400, currency: 'TRY' } }
])
const availableProductKey = ref('FLIGHT-100')
const availableResults = ref<AvailableCampaign[]>([])
const availableLoading = ref(false)

const campaignTypeOptions = [
  { label: 'Discount', value: 0 },
  { label: 'Product Gift', value: 1 },
  { label: 'Gift Coupon', value: 2 }
]

const checkResultRows = computed(() => {
  if (!checkResults.value) return []
  return Object.entries(checkResults.value).map(([key, value]) => ({ key, value }))
})

onMounted(() => {
  void loadCampaigns()
})

function createCampaignDraft(): CampaignForm {
  return {
    id: 0,
    code: '',
    name: '',
    modulId: 1,
    startDate: new Date(),
    endDate: new Date(),
    priority: 0,
    predicate: 'Input.TotalAmount > 100m',
    result: 'Output.TotalDiscount = new Price(Input.TotalAmount * 0.1m, "TRY");',
    description: '',
    quota: null,
    promotionCode: '',
    createDate: new Date(),
    campaignTypes: 0,
    usage: 'true',
    couponCodeId: null,
    departmentId: null,
    cancelReasonId: null,
    cancelSourceId: null
  }
}

function createCampaignInput(): CampaignRuleInputForm {
  return {
    totalAmount: 750,
    customerType: 'VIP',
    orderCount: 2,
    productCount: 3,
    orderTime: new Date(),
    city: 'Istanbul',
    category: 'Electronics',
    usageCount: 0
  }
}

function showNotice(severity: 'success' | 'error' | 'info', text: string) {
  notice.value = { severity, text }
}

function formatDate(value: string) {
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return value
  return date.toLocaleDateString()
}

function mapCampaignToForm(campaign: Campaign): CampaignForm {
  return {
    id: campaign.id,
    code: campaign.code,
    name: campaign.name,
    modulId: campaign.modulId ?? 1,
    startDate: campaign.startDate ? new Date(campaign.startDate) : null,
    endDate: campaign.endDate ? new Date(campaign.endDate) : null,
    priority: campaign.priority,
    predicate: campaign.predicate,
    result: campaign.result,
    description: campaign.description ?? '',
    quota: campaign.quota ?? null,
    promotionCode: campaign.promotionCode ?? '',
    createDate: campaign.createDate ? new Date(campaign.createDate) : null,
    campaignTypes: campaign.campaignTypes ?? 0,
    usage: campaign.usage ?? 'true',
    couponCodeId: campaign.couponCodeId ?? null,
    departmentId: campaign.departmentId ?? null,
    cancelReasonId: campaign.cancelReasonId ?? null,
    cancelSourceId: campaign.cancelSourceId ?? null
  }
}

function buildCampaignInput(formValue: CampaignRuleInputForm) {
  return {
    totalAmount: formValue.totalAmount,
    customerType: formValue.customerType,
    orderCount: formValue.orderCount,
    productCount: formValue.productCount,
    orderTime: formValue.orderTime?.toISOString(),
    city: formValue.city,
    category: formValue.category,
    usageCount: formValue.usageCount
  }
}

async function loadCampaigns() {
  loading.value = true
  notice.value = null
  try {
    const params = new URLSearchParams()
    if (filters.after) params.set('after', filters.after.toISOString())
    if (filters.moduleId !== null) params.set('moduleId', String(filters.moduleId))
    const url = `/api/Campaign${params.toString() ? `?${params.toString()}` : ''}`
    const response = await fetch(url)
    if (!response.ok) throw new Error(await response.text())
    campaigns.value = await response.json()
  } catch (error) {
    showNotice('error', 'Failed to load campaigns.')
  } finally {
    loading.value = false
  }
}

function openCreate() {
  isEdit.value = false
  form.value = createCampaignDraft()
  editorVisible.value = true
}

function openEdit(campaign: Campaign) {
  isEdit.value = true
  form.value = mapCampaignToForm(campaign)
  editorVisible.value = true
}

async function saveCampaign() {
  notice.value = null
  try {
    const payload = {
      id: form.value.id,
      code: form.value.code,
      name: form.value.name,
      modulId: form.value.modulId,
      startDate: form.value.startDate?.toISOString(),
      endDate: form.value.endDate?.toISOString(),
      priority: form.value.priority,
      predicate: form.value.predicate,
      result: form.value.result,
      description: form.value.description || null,
      quota: form.value.quota,
      promotionCode: form.value.promotionCode || null,
      createDate: form.value.createDate?.toISOString(),
      campaignTypes: form.value.campaignTypes,
      usage: form.value.usage || null,
      couponCodeId: form.value.couponCodeId,
      departmentId: form.value.departmentId,
      cancelReasonId: form.value.cancelReasonId,
      cancelSourceId: form.value.cancelSourceId
    }
    if (isEdit.value) {
      const response = await fetch(`/api/Campaign/${payload.id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
      })
      if (!response.ok) throw new Error(await response.text())
      showNotice('success', 'Campaign updated.')
    } else {
      const response = await fetch('/api/Campaign', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
      })
      if (!response.ok) throw new Error(await response.text())
      showNotice('success', 'Campaign created.')
    }
    editorVisible.value = false
    await loadCampaigns()
  } catch (error) {
    showNotice('error', 'Unable to save the campaign.')
  }
}

async function deleteCampaign(campaign: Campaign) {
  notice.value = null
  try {
    const response = await fetch(`/api/Campaign/${campaign.id}`, { method: 'DELETE' })
    if (!response.ok) throw new Error(await response.text())
    showNotice('success', 'Campaign deleted.')
    await loadCampaigns()
  } catch (error) {
    showNotice('error', 'Unable to delete the campaign.')
  }
}

function addCheckItem() {
  checkItems.value.push({ key: '', enabled: false })
}

function removeCheckItem(index: number) {
  checkItems.value.splice(index, 1)
}

async function checkCampaigns() {
  checkLoading.value = true
  notice.value = null
  try {
    const payload: Record<string, boolean> = {}
    checkItems.value.forEach((item) => {
      if (item.key.trim().length) {
        payload[item.key.trim()] = item.enabled
      }
    })
    const response = await fetch('/api/Campaign/check', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload)
    })
    if (!response.ok) throw new Error(await response.text())
    checkResults.value = await response.json()
    showNotice('success', 'Campaign check completed.')
  } catch (error) {
    showNotice('error', 'Campaign check failed.')
  } finally {
    checkLoading.value = false
  }
}

function openQuotaDialog(campaign: Campaign) {
  quotaTarget.value = campaign
  quotaValue.value = campaign.quota ?? 0
  quotaResult.value = null
  quotaDialogVisible.value = true
}

async function checkQuota() {
  if (!quotaTarget.value) return
  notice.value = null
  try {
    const response = await fetch(`/api/Campaign/quota/${quotaTarget.value.id}?quota=${quotaValue.value}`)
    if (!response.ok) throw new Error(await response.text())
    quotaResult.value = await response.json()
  } catch (error) {
    showNotice('error', 'Quota check failed.')
  }
}

async function evaluateCampaigns() {
  evaluationLoading.value = true
  notice.value = null
  try {
    const response = await fetch('/api/Campaign/evaluate', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ input: buildCampaignInput(evaluationInput.value) })
    })
    if (!response.ok) throw new Error(await response.text())
    evaluationResults.value = await response.json()
  } catch (error) {
    showNotice('error', 'Campaign evaluation failed.')
  } finally {
    evaluationLoading.value = false
  }
}

function addProduct() {
  availableProducts.value.push({
    key: `PRODUCT-${availableProducts.value.length + 1}`,
    totalPrice: { value: 100, currency: 'TRY' }
  })
}

function removeProduct(index: number) {
  availableProducts.value.splice(index, 1)
}

async function loadAvailableCampaigns() {
  availableLoading.value = true
  notice.value = null
  try {
    const response = await fetch('/api/Campaign/available', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        productKey: availableProductKey.value,
        input: buildCampaignInput(availableInput.value),
        moduleId: filters.moduleId ?? 1,
        products: availableProducts.value.map((product) => ({
          key: product.key,
          totalPrice: product.totalPrice,
          campaignInformations: {}
        }))
      })
    })
    if (!response.ok) throw new Error(await response.text())
    availableResults.value = await response.json()
  } catch (error) {
    showNotice('error', 'Available campaigns failed.')
  } finally {
    availableLoading.value = false
  }
}
</script>

<template>
  <section class="page-frame">
    <Toolbar class="page-toolbar">
      <template #start>
        <div class="toolbar-title">
          <span>Campaign Management</span>
          <small>Manage campaigns and test execution flows</small>
        </div>
      </template>
      <template #end>
        <Button icon="pi pi-refresh" label="Refresh" text @click="loadCampaigns" />
        <Button icon="pi pi-plus" label="New Campaign" @click="openCreate" />
      </template>
    </Toolbar>

    <div class="filter-bar">
      <div class="form-field">
        <label for="after-date">After Date</label>
        <Calendar id="after-date" v-model="filters.after" dateFormat="yy-mm-dd" />
      </div>
      <div class="form-field">
        <label for="module-id">Module Id</label>
        <InputNumber id="module-id" v-model="filters.moduleId" />
      </div>
      <div class="form-field filter-actions">
        <Button icon="pi pi-filter" label="Apply Filters" @click="loadCampaigns" />
      </div>
    </div>

    <Message v-if="notice" :severity="notice.severity" closable @close="notice = null">
      {{ notice.text }}
    </Message>

    <DataTable :value="campaigns" :loading="loading" dataKey="id" stripedRows class="app-table">
      <Column field="code" header="Code" />
      <Column field="name" header="Name" />
      <Column field="modulId" header="Module" />
      <Column field="priority" header="Priority" />
      <Column field="quota" header="Quota" />
      <Column field="startDate" header="Start">
        <template #body="{ data }">
          <span>{{ formatDate(data.startDate) }}</span>
        </template>
      </Column>
      <Column field="endDate" header="End">
        <template #body="{ data }">
          <span>{{ formatDate(data.endDate) }}</span>
        </template>
      </Column>
      <Column header="Actions" style="width: 18rem">
        <template #body="{ data }">
          <div class="table-actions">
            <Button icon="pi pi-chart-line" text rounded @click="openQuotaDialog(data)" />
            <Button icon="pi pi-pencil" text rounded @click="openEdit(data)" />
            <Button icon="pi pi-trash" text rounded severity="danger" @click="deleteCampaign(data)" />
          </div>
        </template>
      </Column>
    </DataTable>

    <Divider />

    <div class="check-panel">
      <div class="check-header">
        <div>
          <div class="check-title">Campaign Key Check</div>
          <div class="check-sub">Check the activation flags for campaign keys.</div>
        </div>
        <Button icon="pi pi-plus" label="Add Key" text @click="addCheckItem" />
      </div>

      <div class="check-items">
        <div v-for="(item, index) in checkItems" :key="index" class="check-item">
          <InputText v-model="item.key" placeholder="key" />
          <ToggleButton v-model="item.enabled" onLabel="True" offLabel="False" />
          <Button icon="pi pi-times" text severity="danger" @click="removeCheckItem(index)" />
        </div>
      </div>

      <Button :loading="checkLoading" icon="pi pi-bolt" label="Check Campaigns" @click="checkCampaigns" />

      <DataTable v-if="checkResultRows.length" :value="checkResultRows" class="mini-table" dataKey="key">
        <Column field="key" header="Key" />
        <Column field="value" header="Active">
          <template #body="{ data }">
            <Tag :value="data.value ? 'True' : 'False'" :severity="data.value ? 'success' : 'warning'" />
          </template>
        </Column>
      </DataTable>
    </div>

    <Divider />

    <div class="check-panel">
      <div class="check-header">
        <div>
          <div class="check-title">Evaluate Campaigns</div>
          <div class="check-sub">Run rules to see discount outputs.</div>
        </div>
        <Button :loading="evaluationLoading" icon="pi pi-play" label="Evaluate" @click="evaluateCampaigns" />
      </div>

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
          <label for="eval-order-count">Order Count</label>
          <InputNumber id="eval-order-count" v-model="evaluationInput.orderCount" />
        </div>
        <div class="form-field">
          <label for="eval-product-count">Product Count</label>
          <InputNumber id="eval-product-count" v-model="evaluationInput.productCount" />
        </div>
        <div class="form-field">
          <label for="eval-order-time">Order Time</label>
          <Calendar id="eval-order-time" v-model="evaluationInput.orderTime" showIcon showTime hourFormat="24" />
        </div>
        <div class="form-field">
          <label for="eval-city">City</label>
          <InputText id="eval-city" v-model="evaluationInput.city" />
        </div>
        <div class="form-field">
          <label for="eval-category">Category</label>
          <InputText id="eval-category" v-model="evaluationInput.category" />
        </div>
        <div class="form-field">
          <label for="eval-usage">Usage Count</label>
          <InputNumber id="eval-usage" v-model="evaluationInput.usageCount" />
        </div>
      </div>

      <DataTable v-if="evaluationResults.length" :value="evaluationResults" stripedRows>
        <Column field="totalDiscount" header="Total Discount">
          <template #body="{ data }">
            <span>{{ data.totalDiscount.currency }} {{ data.totalDiscount.value }}</span>
          </template>
        </Column>
        <Column field="campaignProductDiscount" header="Product Discount">
          <template #body="{ data }">
            <span v-if="data.campaignProductDiscount">
              {{ data.campaignProductDiscount.productKey }} -
              {{ data.campaignProductDiscount.discountAmount.currency }}
              {{ data.campaignProductDiscount.discountAmount.value }}
            </span>
            <span v-else>-</span>
          </template>
        </Column>
      </DataTable>
    </div>

    <Divider />

    <div class="check-panel">
      <div class="check-header">
        <div>
          <div class="check-title">Available Campaigns</div>
          <div class="check-sub">Resolve campaigns per product key.</div>
        </div>
        <Button :loading="availableLoading" icon="pi pi-search" label="Resolve" @click="loadAvailableCampaigns" />
      </div>

      <div class="form-grid">
        <div class="form-field">
          <label for="avail-product-key">Product Key</label>
          <InputText id="avail-product-key" v-model="availableProductKey" />
        </div>
        <div class="form-field">
          <label for="avail-total">Total Amount</label>
          <InputNumber id="avail-total" v-model="availableInput.totalAmount" />
        </div>
        <div class="form-field">
          <label for="avail-customer">Customer Type</label>
          <InputText id="avail-customer" v-model="availableInput.customerType" />
        </div>
        <div class="form-field">
          <label for="avail-usage">Usage Count</label>
          <InputNumber id="avail-usage" v-model="availableInput.usageCount" />
        </div>
      </div>

      <div class="check-items">
        <div class="check-item">
          <div class="check-title">Products</div>
          <Button icon="pi pi-plus" label="Add Product" text @click="addProduct" />
        </div>
        <div v-for="(product, index) in availableProducts" :key="product.key" class="check-item">
          <InputText v-model="product.key" placeholder="Product Key" />
          <InputNumber v-model="product.totalPrice.value" placeholder="Price" />
          <InputText v-model="product.totalPrice.currency" placeholder="Currency" />
          <Button icon="pi pi-times" text severity="danger" @click="removeProduct(index)" />
        </div>
      </div>

      <DataTable v-if="availableResults.length" :value="availableResults" dataKey="campaignCode" stripedRows>
        <Column field="campaignCode" header="Campaign Code" />
        <Column field="campaignName" header="Campaign Name" />
        <Column field="productKey" header="Product Key" />
        <Column header="Target Discounts">
          <template #body="{ data }">
            <div v-if="data.targetResults.length">
              <div v-for="target in data.targetResults" :key="target.productKey">
                {{ target.productKey }} - {{ target.discount.currency }} {{ target.discount.value }}
              </div>
            </div>
            <span v-else>-</span>
          </template>
        </Column>
      </DataTable>
    </div>
  </section>

  <Dialog v-model:visible="editorVisible" modal header="Campaign Editor" class="dialog-wide">
    <div class="form-grid">
      <div class="form-field">
        <label for="camp-code">Code</label>
        <InputText id="camp-code" v-model="form.code" />
      </div>
      <div class="form-field">
        <label for="camp-name">Name</label>
        <InputText id="camp-name" v-model="form.name" />
      </div>
      <div class="form-field">
        <label for="camp-module">Module Id</label>
        <InputNumber id="camp-module" v-model="form.modulId" />
      </div>
      <div class="form-field">
        <label for="camp-type">Campaign Type</label>
        <Dropdown id="camp-type" v-model="form.campaignTypes" :options="campaignTypeOptions" optionLabel="label" optionValue="value" />
      </div>
      <div class="form-field">
        <label for="camp-priority">Priority</label>
        <InputNumber id="camp-priority" v-model="form.priority" />
      </div>
      <div class="form-field">
        <label for="camp-start">Start Date</label>
        <Calendar id="camp-start" v-model="form.startDate" showIcon />
      </div>
      <div class="form-field">
        <label for="camp-end">End Date</label>
        <Calendar id="camp-end" v-model="form.endDate" showIcon />
      </div>
      <div class="form-field">
        <label for="camp-quota">Quota</label>
        <InputNumber id="camp-quota" v-model="form.quota" />
      </div>
      <div class="form-field">
        <label for="camp-promo">Promotion Code</label>
        <InputText id="camp-promo" v-model="form.promotionCode" />
      </div>
      <div class="form-field">
        <label for="camp-coupon">Coupon Code Id</label>
        <InputNumber id="camp-coupon" v-model="form.couponCodeId" />
      </div>
      <div class="form-field">
        <label for="camp-dept">Department Id</label>
        <InputNumber id="camp-dept" v-model="form.departmentId" />
      </div>
      <div class="form-field">
        <label for="camp-cancel-reason">Cancel Reason Id</label>
        <InputNumber id="camp-cancel-reason" v-model="form.cancelReasonId" />
      </div>
      <div class="form-field">
        <label for="camp-cancel-source">Cancel Source Id</label>
        <InputNumber id="camp-cancel-source" v-model="form.cancelSourceId" />
      </div>
      <div class="form-field full">
        <label for="camp-usage">Usage Expression</label>
        <Textarea id="camp-usage" v-model="form.usage" rows="2" autoResize />
      </div>
      <div class="form-field full">
        <label for="camp-desc">Description</label>
        <Textarea id="camp-desc" v-model="form.description" rows="2" autoResize />
      </div>
      <div class="form-field full">
        <label for="camp-predicate">Predicate</label>
        <Textarea id="camp-predicate" v-model="form.predicate" rows="2" autoResize />
      </div>
      <div class="form-field full">
        <label for="camp-result">Result</label>
        <Textarea id="camp-result" v-model="form.result" rows="2" autoResize />
      </div>
    </div>
    <template #footer>
      <Button label="Cancel" text @click="editorVisible = false" />
      <Button label="Save Campaign" @click="saveCampaign" />
    </template>
  </Dialog>

  <Dialog v-model:visible="quotaDialogVisible" modal header="Quota Check" class="dialog-narrow">
    <div v-if="quotaTarget" class="quota-body">
      <div class="quota-title">{{ quotaTarget.name }}</div>
      <div class="quota-sub">Campaign {{ quotaTarget.code }}</div>
    </div>
    <div class="form-field">
      <label for="quota-value">Quota</label>
      <InputNumber id="quota-value" v-model="quotaValue" />
    </div>
    <div v-if="quotaResult !== null" class="quota-result">
      <Tag :value="quotaResult ? 'Quota OK' : 'Quota Exceeded'" :severity="quotaResult ? 'success' : 'danger'" />
    </div>
    <template #footer>
      <Button label="Close" text @click="quotaDialogVisible = false" />
      <Button label="Check" icon="pi pi-chart-line" @click="checkQuota" />
    </template>
  </Dialog>
</template>
