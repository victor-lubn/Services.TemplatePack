#-----------------------------------------------------------------------
# Terraform backend
#-----------------------------------------------------------------------
terraform {
  backend "azurerm" {
    resource_group_name  = "rg-terraform-tfstate"
    storage_account_name = "sttfstatefilesdev"
    container_name       = "{# Project Name #}dev"
    key                  = "dev.{# Project Name #}.terraform.tfstate"
  }

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "< 3.0.0"
    }
    azapi = {
      source  = "azure/azapi"
      version = ">= 1.0.0"
    }
  }
}
