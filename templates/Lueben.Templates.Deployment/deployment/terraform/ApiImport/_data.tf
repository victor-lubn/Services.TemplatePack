#-----------------------------------------------------------------------
# Current connection details
#-----------------------------------------------------------------------
data "azurerm_client_config" "current" {}



#-----------------------------------------------------------------------
# APIM KeyVault resource
#-----------------------------------------------------------------------
data "azurerm_key_vault" "apim_key_vault" {
  provider            = azurerm.keyvault
  name                = "kvt-${local.short_app_name}-${var.environment}-${local.location}"
  resource_group_name = local.key_vault_resource_group
}



#-----------------------------------------------------------------------
# API scope secret value
#-----------------------------------------------------------------------
data "azurerm_key_vault_secret" "api_scope" {
  provider     = azurerm.keyvault
  name         = "kvs-sp-api-host-${var.environment}-scope"
  key_vault_id = data.azurerm_key_vault.apim_key_vault.id
}
