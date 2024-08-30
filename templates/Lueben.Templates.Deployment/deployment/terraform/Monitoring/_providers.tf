#-----------------------------------------------------------------------
# Providers
#-----------------------------------------------------------------------
provider "azurerm" {
  skip_provider_registration = true
  features {}
}



#-----------------------------------------------------------------------
# KeyVault subscription
#-----------------------------------------------------------------------
provider "azurerm" {
  alias                      = "keyvault"
  subscription_id            = var.key_vault_subscription_id
  skip_provider_registration = true
  features {}
}



#-----------------------------------------------------------------------
# Digital subscription
#-----------------------------------------------------------------------
provider "azurerm" {
  alias                      = "digital"
  subscription_id            = var.digital_subscription_id
  skip_provider_registration = true
  features {}
}
