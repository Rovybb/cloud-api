import "dotenv/config";
import * as http from "node:http";
import { createConnection } from "./data/dbConnection.js";
import handleAPIRoutes from "./routers/apiRouter.js";

await createConnection();

http.createServer(async (req, res) => {
    await handleAPIRoutes(req, res);
}).listen(process.env.PORT, () =>
    console.log(
        `\nServer running on \x1b[34mhttp://localhost:${process.env.PORT}\x1b[0m`
    )
);
