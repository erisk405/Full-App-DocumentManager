import { Role } from "./role.type";


export interface User {
  createAt?: Date;
  email: string;
  first_name: string;
  userId?: string;
  last_name: string;
  phone: string;
  profileImageUrl?: string;
  roleId?: string;
  role_name: string;
  username: string;
}
