import { useState, useEffect } from 'react';
import { Property } from '../types/property';
import { PropertyCard } from './PropertyCard';
import { getFavorites } from '../services/favoriteService';
import { FaHeart, FaSpinner } from 'react-icons/fa';

export const FavoriteProperties = () => {
  const [favorites, setFavorites] = useState<Property[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchFavorites = async () => {
    try {
      setLoading(true);
      const data = await getFavorites();
      setFavorites(data);
      setError(null);
    } catch (err) {
      setError('Failed to fetch favorite properties');
      console.error('Error fetching favorites:', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchFavorites();
  }, []);

  if (loading) {
    return (
      <div className="flex justify-center items-center min-h-[200px]">
        <FaSpinner className="text-4xl text-primary-600 animate-spin" />
      </div>
    );
  }

  if (error) {
    return (
      <div className="text-center py-8">
        <div className="text-red-500 text-5xl mb-4">⚠️</div>
        <p className="text-gray-600">{error}</p>
        <button
          onClick={fetchFavorites}
          className="mt-4 px-4 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700 transition-colors"
        >
          Try Again
        </button>
      </div>
    );
  }

  if (favorites.length === 0) {
    return (
      <div className="text-center py-8">
        <FaHeart className="text-gray-400 text-5xl mx-auto mb-4" />
        <p className="text-gray-600">No favorite properties yet</p>
      </div>
    );
  }

  return (
    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 p-6">
      {favorites.map((property) => (
        <PropertyCard
          key={property.Id}
          property={property}
          onFavoriteChange={fetchFavorites}
          initialFavoriteState={true}
        />
      ))}
    </div>
  );
};
