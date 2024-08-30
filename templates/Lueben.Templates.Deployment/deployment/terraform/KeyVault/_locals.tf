#-----------------------------------------------------------------------
# Local variables
#-----------------------------------------------------------------------
locals {

  long_app_name  = "{# Long App Name #}"
  short_app_name = "{# Short App Name #}"
  location       = "uks"
    
  key_vault_resource_group = "rg-${local.long_app_name}-${var.environment}"

  tags = {
    application = local.long_app_name
    environment = var.environment
  }

}
