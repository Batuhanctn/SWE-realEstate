import { useState, useEffect } from 'react';
import { Property } from '../types/property';
import { FaHeart, FaBed, FaBath, FaRuler, FaMapMarkerAlt } from 'react-icons/fa';
import { addToFavorites, removeFromFavorites, checkIsFavorite } from '../services/favoriteService';
import { useNavigate } from 'react-router-dom';

interface PropertyCardProps {
  property: Property;
  onFavoriteChange?: () => void;
  initialFavoriteState?: boolean;
}

export const PropertyCard = ({ property, onFavoriteChange, initialFavoriteState }: PropertyCardProps) => {
  const [isFavorite, setIsFavorite] = useState(initialFavoriteState || false);
  const [isLoading, setIsLoading] = useState(false);
  const navigate = useNavigate();

  useEffect(() => {
    const checkFavoriteStatus = async () => {
      const token = localStorage.getItem('token');
      if (token && initialFavoriteState === undefined) {
        try {
          const status = await checkIsFavorite(property.Id);
          setIsFavorite(status);
        } catch (error) {
          console.error('Error checking favorite status:', error);
        }
      }
    };
    checkFavoriteStatus();
  }, [property.Id, initialFavoriteState]);

  const handleFavoriteClick = async (e: React.MouseEvent) => {
    e.preventDefault();
    e.stopPropagation();
    
    if (!localStorage.getItem('token')) {
      navigate('/login');
      return;
    }

    setIsLoading(true);
    try {
      if (isFavorite) {
        await removeFromFavorites(property.Id);
      } else {
        await addToFavorites(property.Id);
      }
      setIsFavorite(!isFavorite);
      if (onFavoriteChange) {
        onFavoriteChange();
      }
    } catch (error) {
      console.error('Error updating favorite status:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleCardClick = () => {
    navigate(`/property/${property.Id}`);
  };

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat('tr-TR', {
      style: 'currency',
      currency: 'TRY',
      minimumFractionDigits: 0,
      maximumFractionDigits: 0,
    }).format(price);
  };

  return (
    <div 
      onClick={handleCardClick}
      className="group relative bg-white rounded-2xl shadow-lg overflow-hidden transform transition-all duration-300 hover:-translate-y-1 hover:shadow-xl cursor-pointer"
    >
      {/* Image Section */}
      <div className="relative aspect-[4/3]">
        <img
          src={property.imageUrl || 'https://via.placeholder.com/400x300'}
          alt={property.title}
          className="w-full h-full object-cover"
        />
        <div className="absolute inset-0 bg-gradient-to-t from-black/50 to-transparent" />
        
        {/* Favorite Button */}
        <button
          onClick={handleFavoriteClick}
          disabled={isLoading}
          className={`absolute top-4 right-4 p-2 rounded-full bg-white/90 hover:bg-white transition-colors duration-200 group ${
            isLoading ? 'cursor-wait' : 'cursor-pointer'
          }`}
        >
          <FaHeart
            className={`text-xl transition-colors duration-200 ${
              isFavorite ? 'text-red-500' : 'text-gray-400 group-hover:text-red-500'
            }`}
          />
        </button>

        {/* Property Status */}
        <div className="absolute bottom-4 left-4 flex gap-2">
          <span className="px-3 py-1 text-sm font-semibold text-white bg-primary-600 rounded-full">
            {property.type}
          </span>
          <span className="px-3 py-1 text-sm font-semibold text-white bg-green-600 rounded-full">
            {property.status}
          </span>
        </div>
      </div>

      {/* Content Section */}
      <div className="p-5 space-y-4">
        {/* Title and Price */}
        <div className="space-y-1">
          <div className="flex items-start justify-between gap-2">
            <h3 className="text-lg font-semibold text-gray-900 group-hover:text-primary-600 transition-colors duration-200 line-clamp-2">
              {property.title}
            </h3>
            <p className="text-lg font-bold text-primary-600 whitespace-nowrap">
              {formatPrice(property.price)}
            </p>
          </div>
          <div className="flex items-center text-gray-500">
            <FaMapMarkerAlt className="mr-1 text-primary-500" />
            <p className="text-sm line-clamp-1">{property.location}</p>
          </div>
        </div>

        {/* Property Features */}
        <div className="grid grid-cols-3 gap-4 py-3 border-t border-gray-100">
          <div className="flex items-center text-gray-600">
            <FaBed className="mr-2 text-primary-500" />
            <span className="text-sm">{property.bedrooms} Yatak</span>
          </div>
          <div className="flex items-center text-gray-600">
            <FaBath className="mr-2 text-primary-500" />
            <span className="text-sm">{property.bathrooms} Banyo</span>
          </div>
          <div className="flex items-center text-gray-600">
            <FaRuler className="mr-2 text-primary-500" />
            <span className="text-sm">{property.squareMeters}m²</span>
          </div>
        </div>

        {/* Property Features Tags */}
        <div className="flex flex-wrap gap-2">
          {property.features?.slice(0, 3).map((feature, index) => (
            <span
              key={index}
              className="px-2 py-1 text-xs font-medium text-primary-600 bg-primary-50 rounded-full"
            >
              {feature}
            </span>
          ))}
        </div>
      </div>
    </div>
  );
};
