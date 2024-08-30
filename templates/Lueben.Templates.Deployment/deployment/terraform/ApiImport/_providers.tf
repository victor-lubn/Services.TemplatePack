#-----------------------------------------------------------------------
# Default provider for API import
#-----------------------------------------------------------------------
provider "azurerm" {
  skip_provider_registration = true
  features {}
}



#-----------------------------------------------------------------------
# KeyVault provider for API import
#-----------------------------------------------------------------------
provider "azurerm" {
  alias                      = "keyvault"
  subscription_id            = var.key_vault_subscription_id
  skip_provider_registration = true
  features {}
}
