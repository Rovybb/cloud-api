import handleBookRoutes from './routes/bookRoutes.js';
import handleReviewRoutes from './routes/reviewRoutes.js';

const handleAPIRoutes = async (req, res) => {
    if (req.url.startsWith('/api/book')) {
        handleBookRoutes(req, res);
    } else if (req.url.startsWith('/api/review')) {
        handleReviewRoutes(req, res);
    }
    else {
        res.writeHead(404, { 'Content-Type': 'application/json' });
        res.end(JSON.stringify({ message: 'Route Not Found' }));
    }
};

export default handleAPIRoutes;