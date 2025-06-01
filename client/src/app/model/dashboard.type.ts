export interface sortBy {
    name: string;
    value: string;
}


export interface Country {
    name?: string;
    code?: string;
}

export interface Representative {
    name?: string;
    image?: string;
}


export enum UserRole {
    Admin = "Admin",
    HRAdmin = "HR Admin",    // ค่าที่แสดงมีช่องว่างได้
    SuperAdmin = "Super Admin",
    Employee = "Employee"
}

