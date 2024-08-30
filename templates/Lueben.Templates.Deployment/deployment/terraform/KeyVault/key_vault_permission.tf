#-----------------------------------------------------------------------
# Key Vault default permissions creation
#-----------------------------------------------------------------------
module "key_vault_permission" {

  # Iteration:
  for_each = var.deployment_locations
    
  # Module source link:
  source = "git::git@ssh.dev.azure.com:v3/LuebenJoinery/Lueben/Lueben.Terraform.Azure//modules/Lueben-terraform-key-vault-acc-policy?ref=development"

  # Local variables:
  key_permissions = [
    "Backup",
    "Create",
    "Decrypt",
    "Delete",
    "Encrypt",
    "Get",
    "Import",
    "List",
    "Purge",
    "Recover",
    "Restore",
    "Sign",
    "UnwrapKey",
    "Update",
    "Verify",
    "WrapKey"
  ]
  secret_permissions = [
    "Backup",
    "Delete",
    "Get",
    "List",
    "Purge",
    "Recover",
    "Restore",
    "Set"
  ]
  storage_permissions = [
    "Backup",
    "Delete",
    "DeleteSAS",
    "Get",
    "GetSAS",
    "List",
    "ListSAS",
    "Purge",
    "Recover",
    "RegenerateKey",
    "Restore",
    "Set",
    "SetSAS",
    "Update"
  ]

  # Linked variables:
  key_vault_id = module.key_vault[each.key].key_vault_id
  tenant_id    = data.azurerm_client_config.current.tenant_id
  object_id    = data.azurerm_client_config.current.object_id

  providers = {
    azurerm = azurerm.keyvault
  }

}
