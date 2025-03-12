import sql from "mssql";
import { queryDatabase } from "../data/dbConnection.js";

export const getReviews = async () => {
    const query = 'SELECT * FROM Reviews ORDER BY createdAt DESC';
    return await queryDatabase(query);
};

export const getReviewById = async (id) => {
    const query = 'SELECT * FROM Reviews WHERE id = @id';
    const params = [{ name: 'id', type: sql.Int, value: id }];
    return await queryDatabase(query, params);
};

export const getReviewsByBookId = async (bookId) => {
    const query = 'SELECT * FROM Reviews WHERE bookId = @bookId ORDER BY createdAt DESC';
    const params = [{ name: 'bookId', type: sql.Int, value: bookId }];
    return await queryDatabase(query, params);
};

export const addReview = async(review) => {
    const query = `
        INSERT INTO Reviews (rating, description, bookId, createdAt)
        VALUES (@rating, @description, @bookId, @createdAt);
        SELECT SCOPE_IDENTITY() AS id;
    `;
    const params = [
        { name: 'rating', type: sql.Int, value: review.rating },
        { name: 'description', type: sql.NVarChar, value: review.description },
        { name: 'bookId', type: sql.Int, value: review.bookId },
        { name: 'createdAt', type: sql.DateTime, value: new Date() }
    ];
    await queryDatabase(query, params);
};

export const updateReview = async (id, updatedReview) => {
    const query = `
        UPDATE Reviews
        SET rating = @rating, description = @description, bookId = @bookId
        WHERE id = @id
    `;
    const params = [
        { name: 'rating', type: sql.Int, value: updatedReview.rating },
        { name: 'description', type: sql.NVarChar, value: updatedReview.description },
        { name: 'bookId', type: sql.Int, value: updatedReview.bookId },
        { name: 'id', type: sql.Int, value: id }
    ];
    await queryDatabase(query, params);
};

export const deleteReview = async (id) => {
    const query = 'DELETE FROM Reviews WHERE id = @id';
    const params = [{ name: 'id', type: sql.Int, value: id }];
    await queryDatabase(query, params);
};
