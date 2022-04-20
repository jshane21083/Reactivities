import { User } from "./users";

export interface Profile {
    username: string;
    displayName: string;
    image?: string;
    bio?: string
}

export class Profile implements Profile {
    constructor(user: User) {
        this.username = this.username;
        this.displayName = this.displayName;
        this.image = this.image;
    }
}