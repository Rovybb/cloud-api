/**
 * Utility function to log requests in the console
 * 
 * Use example:
 * ```
 * requestLogger(req.method, req.url, 200);
 * ```
 * @param {string} method Html method. ex.: GET, POST, PUT, DELETE
 * @param {string} url Requested url
 * @param {number} statusCode Response status code
 */
const requestLogger = (method, url, statusCode) => {
    const date = new Date().toTimeString().split(" ")[0];
    const coloredMethod = (mtd) => {
        switch (mtd) {
            case "GET":
                return `\x1b[32m${mtd}\x1b[0m`;
            case "POST":
                return `\x1b[33m${mtd}\x1b[0m`;
            case "PUT":
                return `\x1b[34m${mtd}\x1b[0m`;
            case "DELETE":
                return `\x1b[31m${mtd}\x1b[0m`;
            default:
                return mtd;
        }
    };
    const coloredStatusCode = (code) => {
        if (code >= 200 && code < 300) {
            return `\x1b[32m${code}\x1b[0m`;
        } else if (code >= 300 && code < 400) {
            return `\x1b[33m${code}\x1b[0m`;
        } else if (code >= 400 && code < 600) {
            return `\x1b[31m${code}\x1b[0m`;
        } else {
            return `\x1b[34m${code}\x1b[0m`;
        }
    };
    console.log(`[${date}] ${coloredMethod(method)} ${url} ${coloredStatusCode(statusCode)}`);
};

export default requestLogger;