#-----------------------------------------------------------------------
# Providers configuration
#-----------------------------------------------------------------------
provider "azurerm" {
  skip_provider_registration = true
  features {}
}

provider "azurerm" {
  alias                      = "keyvault"
  subscription_id            = var.key_vault_subscription_id
  skip_provider_registration = true
  features {}
}
