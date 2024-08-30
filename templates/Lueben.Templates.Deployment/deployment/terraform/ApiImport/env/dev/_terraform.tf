#-----------------------------------------------------------------------
# Terraform backend
#-----------------------------------------------------------------------
terraform {
  backend "azurerm" {
    resource_group_name  = "rg-apimanagement-dev"
    storage_account_name = "stapitfstatesdev"
    container_name       = ""
    key                  = ""
  }

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=3.8.0"
    }
  }
}
