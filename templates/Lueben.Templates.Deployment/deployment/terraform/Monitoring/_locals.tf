#-----------------------------------------------------------------------
# Local variables
#-----------------------------------------------------------------------
locals {

  long_app_name               = "{# Long App Name #}"
  short_app_name              = "{# Short App Name #}"
  environment                 = var.environment
  location_uks                = "uks"
  location_ukw                = "ukw"
  app_insights_location       = "UK South"
  resource_group              = "rg-${local.long_app_name}-${var.environment}"
  app_insights_resource_group = "rg-appi-Luebencom-${var.environment}"

  tags = {
    application = local.long_app_name
    environment = var.environment
  }

  dashboard_tags = merge(local.tags, { "hidden-title" = "${upper(local.short_app_name)} Dashboard ${upper(var.environment)}" })
}
