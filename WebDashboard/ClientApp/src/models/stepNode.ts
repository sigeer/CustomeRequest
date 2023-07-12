import { ActionOption } from "./actionOption";

export class StepNode implements IStepNode {
    actionOptionId: number;
    params: string[];
    result: any;

    actionOption: ActionOption | null = null;

    constructor(json: any | null = null) {
        json = json ?? {};
        this.actionOptionId = json.actionOptionId;
        this.params = json.params ?? [];
        this.result = json.result ?? null;
    }

    setActionOption(options: ActionOption[]): void {
        this.actionOption = options.find(x => x.id === this.actionOptionId) ?? null;
    }
}

export interface IStepNode {
    actionOptionId: number;
    params: string[];
    result: any;
    actionOption: ActionOption | null;

    setActionOption(options: ActionOption[]): void;
}