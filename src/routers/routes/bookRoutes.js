import {
    getBook,
    addBook,
    updateBook,
    deleteBook,
    searchBooks,
} from "../../controllers/bookController.js";
import requestLogger from "../../utils/requestLogger.js";

const handleBookRoutes = (req, res) => {
    if (req.url.match(/\/api\/book\/([0-9]+)/) && req.method === "GET") {
        const id = req.url.split("/")[3];
        getBook(req, res, id);
    } else if (
        req.url.match(
            /\/api\/book(\?(search|genre)=.*(&(search|genre)=.*)?)?/
        ) &&
        req.method === "GET"
    ) {
        const params = req.url.split("?")[1]?.split("&");
        const search = params?.find((param) => param.includes("search"));
        const genre = params?.find((param) => param.includes("genre"));
        searchBooks(
            req,
            res,
            search ? search.split("=")[1] : "",
            genre ? genre.split("=")[1] : ""
        );
    } else if (req.url === "/api/book" && req.method === "POST") {
        addBook(req, res);
    } else if (req.url.match(/\/api\/book\/([0-9]+)/) && req.method === "PUT") {
        const id = req.url.split("/")[3];
        updateBook(req, res, id);
    } else if (
        req.url.match(/\/api\/book\/([0-9]+)/) &&
        req.method === "DELETE"
    ) {
        const id = req.url.split("/")[3];
        deleteBook(req, res, id);
    } else {
        res.writeHead(404, { "Content-Type": "application/json" });
        res.end(JSON.stringify({ message: "Book Route Not Found" }));
        requestLogger(req.method, req.url, 404);
    }
};

export default handleBookRoutes;
