#-----------------------------------------------------------------------
# Key Vault Data source (only UKS region)
#-----------------------------------------------------------------------
data "azurerm_key_vault" "key_vault" {
  provider            = azurerm.keyvault
  name                = "kvt-${local.short_app_name}-${var.environment}-${local.location_uks}"
  resource_group_name = local.resource_group
}



#-----------------------------------------------------------------------
# Application Insight Data source (only UKS region)
#-----------------------------------------------------------------------
data "azurerm_application_insights" "app_insights" {
  name                = "appi-${var.environment}-${local.location_uks}"
  resource_group_name = local.app_insights_resource_group
}
