export function json2Obj (jsonStr: string, depth: number = 0): any {
    let preStr = "";
    let preObj : { [key: string]: string } = {};
    let keyDic = []
    let symbolStatck: string[] = [];
    for (let i = 0; i < jsonStr.length; i++) {
        const charVal = jsonStr[i];
        if (isEmptyChar(charVal)) {
            if (depth > 0)
                preStr += charVal;
            continue;
        }
        else if (symbolStatck.length === 0 ? isStartSymbol(charVal) : charVal === "[") {
            if (depth !== 0)
                preStr += charVal;
            symbolStatck.push(charVal);
        }
        else if (isEndSymbol(charVal)) {
            preStr += charVal;
            symbolStatck.pop();

            if (symbolStatck.length === 0 && depth > 0)
                return {lastValue: preStr, lastIndex: i};
        }
        else if (charVal === ",") {
            if (symbolStatck.length === 0) {
                if (depth > 0)
                    return {lastValue: preStr, lastIndex: i};
                else
                    preStr += charVal;
            } else {
                if (depth > 0)
                    preStr += charVal;
            }
        }
        else if (charVal === ":") {
            keyDic.push(preStr);
            const {lastValue, lastIndex} = json2Obj(jsonStr.substring(i + 1, jsonStr.length - 1), depth + 1)
            preObj[preStr] =  lastValue.trim();
            i = i + lastIndex + 1;
            preStr = "";
        }
        else {
            preStr += charVal;
        }
    }
    return preObj
}

function isStartSymbol(s: string) {
    return s === "{" || s === "[";
}

function isEndSymbol(s: string) {
    return s === "}" || s === "]";
}

function isEmptyChar(s: string) {
    return s === "" || s === "\r" || s === "\n" || s === " ";
}