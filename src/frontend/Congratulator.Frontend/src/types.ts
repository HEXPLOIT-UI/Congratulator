export interface LoginUserRequest {
    login: string;
    password: string;
}

export interface CreateUserRequest {
    firstName: string;
    lastName: string;
    login: string;
    password: string;
}

export interface UserDTO {
    entityId: string;
    firstName: string;
    lastName: string;
    role: string;
    login: string;
}

export interface BirthdayDTO {
    entityId: string;
    userId: string;
    firstName: string;
    lastName: string;
    dateOfBirth: string;
    photoPath: string;
    isActive: boolean;
}

export interface ResultWithPaginationOfBirthdayDTO {
    result: BirthdayDTO[];
    availablePages: number;
}