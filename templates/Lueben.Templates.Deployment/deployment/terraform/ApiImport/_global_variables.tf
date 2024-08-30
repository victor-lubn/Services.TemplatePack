#-----------------------------------------------------------------------
# General variables
#-----------------------------------------------------------------------
variable "environment" {
  type        = string
  description = "Mandatory. Environment for resources deployment."
}

variable "digital_subscription_id" {
  type        = string
  description = "Digital subscription ID."
}

variable "key_vault_subscription_id" {
  type        = string
  description = "Key Vault subscription ID."
}
