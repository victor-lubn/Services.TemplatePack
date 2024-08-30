#-----------------------------------------------------------------------
# Local Terraform variables to use by all modules
#-----------------------------------------------------------------------
locals {

  long_app_name  = "{# Long App Name #}"
  short_app_name = "{# Short App Name #}"
  location       = "uks"

  microservices_resource_group = "rg-${local.long_app_name}-${var.environment}"
  key_vault_resource_group     = "rg-${local.long_app_name}-${var.environment}"
  app_insights_resource_group  = "rg-appi-Luebencom-${var.environment}"

  microservices_key_vault_name = "kvt-${local.short_app_name}-${var.environment}-${local.location}"

  tags = {
    application = local.long_app_name
    environment = var.environment
  }
  function_app_tags = {
    "hidden-link: /app-insights-conn-string"         = data.azurerm_application_insights.app_insights.connection_string
    "hidden-link: /app-insights-instrumentation-key" = data.azurerm_application_insights.app_insights.instrumentation_key
    "hidden-link: /app-insights-resource-id"         = data.azurerm_application_insights.app_insights.id
  }

}
