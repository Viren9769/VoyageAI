export interface JwtPayload {

    sub: string;

    email: string;

    given_name: string;

    family_name: string;

    exp: number;

    iss: string;

    aud: string;

}