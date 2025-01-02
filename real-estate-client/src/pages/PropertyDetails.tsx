import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { FaHome, FaBed, FaBath, FaRuler, FaMapMarkerAlt, FaPhone, FaEnvelope, FaWhatsapp, FaHeart } from 'react-icons/fa';
import { Property } from '../types/property';
import { getProperty } from '../services/propertyService';
import { addToFavorites, removeFromFavorites, checkIsFavorite } from '../services/favoriteService';

/**
 * PropertyDetails component displays detailed information about a specific property
 * Allows users to view property details and manage favorites
 * @returns {JSX.Element} The rendered PropertyDetails component
 */
export const PropertyDetails = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [property, setProperty] = useState<Property | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [isFavorite, setIsFavorite] = useState(false);
  const [selectedImage, setSelectedImage] = useState<string>('');

  useEffect(() => {
    /**
     * Fetches property details and favorite status from the API
     * @returns {Promise<void>}
     */
    const fetchProperty = async () => {
      try {
        if (!id) return;
        const data = await getProperty(id);
        setProperty(data);
        setSelectedImage(data.imageUrl);
        
        // Check if property is in favorites
        const token = localStorage.getItem('token');
        if (token) {
          const favoriteStatus = await checkIsFavorite(id);
          setIsFavorite(favoriteStatus);
        }
      } catch (err) {
        setError('Failed to fetch property details');
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    fetchProperty();
  }, [id]);

  /**
   * Handles adding/removing property from favorites
   * @returns {Promise<void>} A promise that resolves when the favorite status is updated
   */
  const handleFavoriteClick = async () => {
    if (!property) return;
    
    const token = localStorage.getItem('token');
    if (!token) {
      navigate('/login');
      return;
    }

    try {
      if (isFavorite) {
        await removeFromFavorites(property.Id);
      } else {
        await addToFavorites(property.Id);
      }
      setIsFavorite(!isFavorite);
    } catch (error) {
      console.error('Error updating favorite status:', error);
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-primary-600"></div>
      </div>
    );
  }

  if (error || !property) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-center">
          <h2 className="text-2xl font-bold text-gray-900 mb-2">Error Loading Property</h2>
          <p className="text-gray-600">{error || 'Property not found'}</p>
          <button
            onClick={() => navigate(-1)}
            className="mt-4 px-4 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700"
          >
            Go Back
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 py-12">
      <div className="container mx-auto px-4 max-w-7xl">
        {/* Property Header */}
        <div className="mb-8">
          <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
            <div>
              <h1 className="text-3xl font-bold text-gray-900 mb-2">{property.title}</h1>
              <div className="flex items-center text-gray-600">
                <FaMapMarkerAlt className="mr-2" />
                <span>{property.location}</span>
              </div>
            </div>
            <div className="flex items-center gap-4">
              <span className="text-3xl font-bold text-primary-600">
                ${property.price.toLocaleString()}
              </span>
              <button
                onClick={handleFavoriteClick}
                className={`p-3 rounded-full ${
                  isFavorite 
                    ? 'bg-red-50 text-red-500' 
                    : 'bg-gray-100 text-gray-400 hover:text-red-500 hover:bg-red-50'
                } transition-colors`}
              >
                <FaHeart className="text-xl" />
              </button>
            </div>
          </div>
        </div>

        {/* Image Gallery */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-8">
          <div className="md:col-span-2">
            <img
              src={selectedImage}
              alt={property.title}
              className="w-full h-[500px] object-cover rounded-lg"
            />
          </div>
          <div className="grid grid-cols-2 md:grid-cols-1 gap-4">
            {property.imageUrls?.slice(0, 4).map((image, index) => (
              <img
                key={index}
                src={image}
                alt={`${property.title} ${index + 1}`}
                className="w-full h-[120px] object-cover rounded-lg cursor-pointer hover:opacity-90 transition-opacity"
                onClick={() => setSelectedImage(image)}
              />
            ))}
          </div>
        </div>

        {/* Property Details */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
          {/* Main Content */}
          <div className="md:col-span-2 space-y-8">
            {/* Overview */}
            <div className="bg-white rounded-xl p-6 shadow-sm">
              <h2 className="text-2xl font-semibold mb-6">Overview</h2>
              <div className="grid grid-cols-2 md:grid-cols-4 gap-6">
                <div className="flex items-center gap-3">
                  <FaBed className="text-2xl text-primary-600" />
                  <div>
                    <p className="text-sm text-gray-500">Bedrooms</p>
                    <p className="font-semibold">{property.bedrooms}</p>
                  </div>
                </div>
                <div className="flex items-center gap-3">
                  <FaBath className="text-2xl text-primary-600" />
                  <div>
                    <p className="text-sm text-gray-500">Bathrooms</p>
                    <p className="font-semibold">{property.bathrooms}</p>
                  </div>
                </div>
                <div className="flex items-center gap-3">
                  <FaRuler className="text-2xl text-primary-600" />
                  <div>
                    <p className="text-sm text-gray-500">Area</p>
                    <p className="font-semibold">{property.squareMeters}m²</p>
                  </div>
                </div>
                <div className="flex items-center gap-3">
                  <FaHome className="text-2xl text-primary-600" />
                  <div>
                    <p className="text-sm text-gray-500">Type</p>
                    <p className="font-semibold">{property.propertyType}</p>
                  </div>
                </div>
              </div>
            </div>

            {/* Description */}
            <div className="bg-white rounded-xl p-6 shadow-sm">
              <h2 className="text-2xl font-semibold mb-4">Description</h2>
              <p className="text-gray-600 leading-relaxed">{property.description}</p>
            </div>

            {/* Features */}
            <div className="bg-white rounded-xl p-6 shadow-sm">
              <h2 className="text-2xl font-semibold mb-6">Features</h2>
              <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
                {property.features?.map((feature, index) => (
                  <div key={index} className="flex items-center gap-2">
                    <div className="w-2 h-2 rounded-full bg-primary-600"></div>
                    <span className="text-gray-600">{feature}</span>
                  </div>
                ))}
              </div>
            </div>
          </div>

          {/* Contact Section */}
          <div className="md:col-span-1">
            <div className="bg-white rounded-xl p-6 shadow-sm sticky top-4">
              <h2 className="text-xl font-semibold mb-6">Contact Agent</h2>
              <div className="space-y-6">
                <button className="w-full px-4 py-3 bg-primary-600 text-white rounded-lg hover:bg-primary-700 flex items-center justify-center gap-2">
                  <FaPhone />
                  Call Agent
                </button>
                <button className="w-full px-4 py-3 bg-green-600 text-white rounded-lg hover:bg-green-700 flex items-center justify-center gap-2">
                  <FaWhatsapp />
                  WhatsApp
                </button>
                <button className="w-full px-4 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 flex items-center justify-center gap-2">
                  <FaEnvelope />
                  Email Agent
                </button>

                <div className="border-t pt-6">
                  <h3 className="font-semibold mb-4">Schedule a Tour</h3>
                  <input
                    type="date"
                    className="w-full px-4 py-2 border rounded-lg mb-4"
                    min={new Date().toISOString().split('T')[0]}
                  />
                  <button className="w-full px-4 py-3 bg-primary-600 text-white rounded-lg hover:bg-primary-700">
                    Request Tour
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};
