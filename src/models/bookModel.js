import sql from "mssql";
import { queryDatabase } from "../data/dbConnection.js";

export const searchBooks = async (search, genre) => {
    const query = `
            SELECT 
                b.id,
                b.title,
                b.author,
                b.genre,
                b.rating,
                b.description
            FROM Books b
            WHERE ( b.title LIKE @search OR b.author LIKE @search ) ${
                genre ? "AND b.genre = @genre" : ""
            }
        `;
    const params = [
        { name: "search", type: sql.NVarChar, value: `%${search}%` },
        { name: "genre", type: sql.NVarChar, value: genre },
    ];
    const response = await queryDatabase(query, params);
    return response.map((book) => {
        const { popularity, ...rest } = book;
        return rest;
    });
};

export const getBookById = async (id) => {
    const query = "SELECT * FROM Books WHERE id = @id";
    const params = [{ name: "id", type: sql.Int, value: id }];
    return await queryDatabase(query, params);
};

export const addBook = async (book) => {
    const query = `
        INSERT INTO Books (title, author, genre, rating, description)
        VALUES (@title, @author, @genre, @rating, @description);
        SELECT SCOPE_IDENTITY() AS id;
    `;
    const params = [
        { name: "title", type: sql.NVarChar, value: book.title },
        { name: "author", type: sql.NVarChar, value: book.author },
        { name: "genre", type: sql.NVarChar, value: book.genre },
        { name: "rating", type: sql.Float, value: 0.0 },
        { name: "description", type: sql.NVarChar, value: book.description },
    ];
    return await queryDatabase(query, params);
};

export const updateBook = async (id, updatedBook) => {
    const query = `
        UPDATE Books
        SET title = @title, author = @author, genre = @genre,
            description = @description
        WHERE id = @id
    `;
    const params = [
        { name: "title", type: sql.NVarChar, value: updatedBook.title },
        { name: "author", type: sql.NVarChar, value: updatedBook.author },
        { name: "genre", type: sql.NVarChar, value: updatedBook.genre },
        {
            name: "description",
            type: sql.NVarChar,
            value: updatedBook.description,
        },
        { name: "id", type: sql.Int, value: id },
    ];
    await queryDatabase(query, params);
};

export const updateBookRating = async (id) => {
    const query = `
        UPDATE Books
        SET rating = (
            SELECT AVG(rating)
            FROM Reviews
            WHERE bookId = @id
        )
        WHERE id = @id
    `;
    const params = [{ name: "id", type: sql.Int, value: id }];
    await queryDatabase(query, params);
};

export const deleteBook = async (id) => {
    const query = "DELETE FROM Books WHERE id = @id";
    const params = [{ name: "id", type: sql.Int, value: id }];
    await queryDatabase(query, params);
};
