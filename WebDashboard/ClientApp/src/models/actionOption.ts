export class ActionOption {
    id: number;
    name: string;
    fromType: string;
    returnType: string;
    params: string[];

    constructor(id: number, name: string, fromType: string, returnType: string, params: string[]) {
        this.id = id;
        this.name = name;
        this.fromType = fromType;
        this.returnType = returnType;
        this.params = params;
    }
}