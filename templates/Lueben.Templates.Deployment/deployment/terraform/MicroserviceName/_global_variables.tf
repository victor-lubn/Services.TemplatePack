#-----------------------------------------------------------------------
# Global variables
#-----------------------------------------------------------------------
variable "environment" {
  type        = string
  description = "Mandatory. Environment for resources deployment."
}



#-----------------------------------------------------------------------
# Providers variables
#-----------------------------------------------------------------------
variable "key_vault_subscription_id" {
  type        = string
  description = "Key Vault subscription ID."
}
