param resourceName string = 'inventorymanagement'
param location string = 'West US 2'

var appServicePlan = {
  name: 'asplan-${resourceName}'
  location: location
}

resource asplan 'Microsoft.Web/serverfarms@2018-02-01' = {
  name: appServicePlan.name
  location: appServicePlan.location
  kind: 'app'
  sku: {
    name: 'S1'
    tier: 'Standard'
    size: 'S1'
    family: 'S'
    capacity: 1
  }
}

var appService = {
  name: 'apiapp-${resourceName}'
  location: location
}

resource apiapp 'Microsoft.Web/sites@2018-11-01' = {
  name: appService.name
  location: appService.location
  kind: 'app'
  properties: {
    serverFarmId: asplan.id
    httpsOnly: true
  }
}

var apiManagement = {
  name: 'apim-${resourceName}'
  location: location
  productName: 'development'
}

resource apim 'Microsoft.ApiManagement/service@2020-12-01' = {
  name: apiManagement.name
  location: apiManagement.location
  sku: {
    name: 'Consumption'
    capacity: 0
  }
  properties: {
    publisherEmail: 'crystal@vanarsdel.com'
    publisherName: 'VanArsdel'
    notificationSenderEmail: 'apimgmt-noreply@vanarsdel.com'
  }
}

resource apimProduct 'Microsoft.ApiManagement/service/products@2020-12-01' = {
  name: '${apim.name}/${apiManagement.productName}'
  properties: {
    displayName: 'Development'
    description: 'For development'
    subscriptionRequired: true
    approvalRequired: false
    state: 'published'
  }
}
