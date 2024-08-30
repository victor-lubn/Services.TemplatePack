#-----------------------------------------------------------------------
# Key Vault creation
#-----------------------------------------------------------------------
module "key_vault" {

  # Iteration:
  for_each = var.deployment_locations

  # Module source link:
  source = "git::git@ssh.dev.azure.com:v3/LuebenJoinery/Lueben/Lueben.Terraform.Azure//modules/Lueben-terraform-key-vault-3.0?ref=development"
    
  # Global variables:
  resource_group_name = local.key_vault_resource_group
  tags                = local.tags

  # Local variables:
  location                        = each.value
  sku_name                        = "standard"
  enabled_for_deployment          = false
  enabled_for_disk_encryption     = false
  enabled_for_template_deployment = false
  enable_rbac_authorization       = false
  purge_protection_enabled        = false
  soft_delete_retention_days      = 90
  default_access_policy           = false
  enable_network_acls             = true
  bypass                          = "AzureServices"
  default_action                  = "Allow"
  enable_contact                  = false

  # Composite variables:
  name = "kvt-${local.short_app_name}-${var.environment}-${each.key}"

  # Linked variables:
  tenant_id = data.azurerm_client_config.current.tenant_id
    
  providers = {
    azurerm = azurerm.keyvault
  }

}
