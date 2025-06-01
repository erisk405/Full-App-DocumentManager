
export interface Role {
  roleId?: string;
  roleName: string;
  permission: permission
}
export interface permission {
  permissionId: string;
  isReadable: boolean;
  isWriteable: boolean;
  isDeleteable: boolean;

}
