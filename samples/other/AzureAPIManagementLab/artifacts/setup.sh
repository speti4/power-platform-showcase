#!/bin/bash

RESOURCE_NAME="InventoryManagement-$(openssl rand -hex 2)"
RESOURCE_GROUP_NAME=$(az group list --query "sort_by([], &name)[0].name" -o tsv)

printf "Provisioning Resources ... (1/4)\n\n"

# Provision Resources
az deployment group create \
    --resource-group $RESOURCE_GROUP_NAME \
    --name powerappsexcercise \
    --template-file ./azuredeploy.json \
    --parameters resourceName=$RESOURCE_NAME \
    --parameters location=westus2 \
    --verbose


printf "Deploying Web API ... (2/4)\n\n"

# Deploy Web API
az webapp deploy \
    --resource-group $RESOURCE_GROUP_NAME \
    --name apiapp-$RESOURCE_NAME \
    --src-path ./publish.zip \
    --type zip \
    --verbose


printf "Importing Web API to Azure API Management ... (3/4)\n\n"

# Import Web API to API Management
az apim api import \
    --resource-group $RESOURCE_GROUP_NAME \
    --service-name apim-$RESOURCE_NAME \
    --api-id 'inventory-management' \
    --display-name 'Inventory Management' \
    --path inventory \
    --api-type http \
    --protocols https \
    --service-url https://apiapp-$RESOURCE_NAME.azurewebsites.net \
    --specification-format OpenApiJson \
    --specification-path ./openapi.json \
    --subscription-required true \
    --verbose


printf "Linking API with Product ... (4/4)\n\n"

# Link API with Product
az apim product api add \
    --resource-group $RESOURCE_GROUP_NAME \
    --service-name apim-$RESOURCE_NAME \
    --product-id development \
    --api-id 'inventory-management' \
    --verbose


printf "Setup complete!\n\n"
printf "***********************   IMPORTANT INFO   *********************\n\n"
printf "Deployed Resource Group: $RESOURCE_GROUP_NAME\n\n"
printf "Web API URL: https://apiapp-$RESOURCE_NAME.azurewebsites.net\n\n"
printf "OpenAPI UI URL: https://apiapp-$RESOURCE_NAME.azurewebsites.net/swagger\n\n"
