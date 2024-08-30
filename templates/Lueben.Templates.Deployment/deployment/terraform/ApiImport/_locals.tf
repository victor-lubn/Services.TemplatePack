#-----------------------------------------------------------------------
# Local Terraform variables to use by all modules
#-----------------------------------------------------------------------
locals {

  long_app_name      = "{# Long App Name #}"
  short_app_name     = "{# Short App Name #}"
  short_project_name = "{# Short Project Name #}"
  location           = "{# Location Suffix #}"

  apim_resource_group      = "rg-${local.long_app_name}-${var.environment}"
  key_vault_resource_group = "rg-${local.long_app_name}-${var.environment}"
  apim_name                = var.environment == "epamqa" ? "apim-${var.environment}-${local.location}" : "apim-Lueben-${var.environment}-${local.location}"
  apim_logger_id           = "/subscriptions/${data.azurerm_client_config.current.subscription_id}/resourceGroups/rg-${local.long_app_name}-${var.environment}/providers/Microsoft.ApiManagement/service/${local.short_app_name}-Lueben-${var.environment}-${local.location}/loggers/${local.short_app_name}-log-${var.environment}-${local.location}"
  Lueben_product_id       = "Lueben"

}
