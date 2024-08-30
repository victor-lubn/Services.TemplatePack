#-----------------------------------------------------------------------
# Global variables
#-----------------------------------------------------------------------
variable "environment" {
  type        = string
  description = "Mandatory. Environment for resources deployment."
}

variable "key_vault_subscription_id" {
  type        = string
  description = "Key Vault subscription id"
}



#-----------------------------------------------------------------------
# Common deployment variables
#-----------------------------------------------------------------------
variable "deployment_locations" {
  type        = map(any)
  default     = {}
  description = "Map of locations where resources should be created."
}
