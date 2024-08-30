#-----------------------------------------------------------------------
# Terraform backend
#-----------------------------------------------------------------------
terraform {
  backend "azurerm" {
    resource_group_name  = "rg-apimanagement-preprd"
    storage_account_name = "stapitfstatespreprd"
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
