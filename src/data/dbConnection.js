import sql from 'mssql';

const config = {
    user: process.env.DB_USER,
    password: process.env.DB_PASS,
    database: process.env.DB_NAME,
    server: process.env.DB_SERVER,
    options: {
        trustServerCertificate: true,
    }
};

const createConnection = async () => {
    try {
        await sql.connect(config);
        console.log("\x1b[32mDatabase connected successfully\x1b[0m");
    } catch (error) {
        console.error("\x1b[31mDatabase connection failed\x1b[0m", error);
    }
};

async function queryDatabase(query, params = []) {
    try {
        let request = new sql.Request();
        params.forEach(param => {
            request.input(param.name, param.type, param.value);
        });
        let result = await request.query(query);
        return result.recordset;
    } catch (err) {
        throw new Error(err.message);
    }
}

export { createConnection, queryDatabase };