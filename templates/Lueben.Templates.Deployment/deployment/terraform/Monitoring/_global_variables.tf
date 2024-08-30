#-----------------------------------------------------------------------
# Global variables
#-----------------------------------------------------------------------
variable "environment" {
  type        = string
  description = "Mandatory. Environment for resources deployment."
}



#-----------------------------------------------------------------------
# KeyVault subscription variables
#-----------------------------------------------------------------------
variable "key_vault_subscription_id" {
  type        = string
  description = "Key Vault subscription ID."
}



#-----------------------------------------------------------------------
# Digital subscription varaibles
#-----------------------------------------------------------------------
variable "digital_subscription_id" {
  type        = string
  description = "Digital subscription ID."
}
