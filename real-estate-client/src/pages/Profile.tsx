import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { FaSignOutAlt, FaHome, FaHeart, FaList, FaEnvelope, FaPhone, FaCalendar } from 'react-icons/fa';
import { FavoriteProperties } from '../components/FavoriteProperties';

/**
 * User interface
 * @typedef {Object} User
 * @property {string} id - User ID
 * @property {string} email - User email
 * @property {string} firstName - User first name
 * @property {string} lastName - User last name
 * @property {string} phoneNumber - User phone number
 * @property {string} createdAt - User creation date
 */
interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  createdAt: string;
}

/**
 * Property interface
 * @typedef {Object} Property
 * @property {string} Id - Property ID
 * @property {string} title - Property title
 * @property {number} price - Property price
 * @property {string} location - Property location
 * @property {string} propertyType - Property type
 * @property {string} status - Property status
 * @property {string} imageUrl - Property image URL
 */
interface Property {
  Id: string;
  title: string;
  price: number;
  location: string;
  propertyType: string;
  status: string;
  imageUrl: string;
}

/**
 * Profile component for displaying and managing user profile information
 * Shows user's properties and favorite listings
 * @returns {JSX.Element} The rendered Profile component
 */
export const Profile = () => {
  const navigate = useNavigate();
  const [user, setUser] = useState<User | null>(null);
  const [properties, setProperties] = useState<Property[]>([]);
  const [activeTab, setActiveTab] = useState<'favorites' | 'listings'>('favorites');

  /**
   * Effect hook to fetch user data and properties on component mount
   */
  useEffect(() => {
    const userData = localStorage.getItem('user');
    if (!userData) {
      navigate('/login');
      return;
    }
    setUser(JSON.parse(userData));
    fetchUserProperties();
  }, [navigate]);

  /**
   * Fetches user's properties from the API
   * @returns {Promise<void>}
   */
  const fetchUserProperties = async () => {
    try {
      const token = localStorage.getItem('token');
      if (!token) return;

      const response = await fetch('http://localhost:5150/api/properties/user', {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      if (!response.ok) throw new Error('Failed to fetch properties');

      const data = await response.json();
      setProperties(data);
    } catch (error) {
      console.error('Error fetching properties:', error);
    }
  };

  /**
   * Handles user logout
   * Clears authentication tokens and redirects to login page
   */
  const handleLogout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    navigate('/login');
  };

  if (!user) return null;

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Hero Section with User Info */}
      <div className="bg-gradient-to-r from-primary-600 to-primary-800 text-white">
        <div className="container mx-auto px-4 py-12">
          <div className="max-w-7xl mx-auto">
            <div className="flex flex-col md:flex-row items-center justify-between gap-8">
              <div className="flex items-center gap-6">
                <div className="w-24 h-24 bg-white rounded-full flex items-center justify-center shadow-lg">
                  <span className="text-3xl font-bold text-primary-600">
                    {user?.firstName?.[0]}{user?.lastName?.[0]}
                  </span>
                </div>
                <div>
                  <h1 className="text-3xl font-bold mb-2">
                    {user?.firstName} {user?.lastName}
                  </h1>
                  <div className="flex flex-col gap-2 text-primary-100">
                    <div className="flex items-center gap-2">
                      <FaEnvelope className="text-primary-200" />
                      <span>{user?.email}</span>
                    </div>
                    <div className="flex items-center gap-2">
                      <FaPhone className="text-primary-200" />
                      <span>{user?.phoneNumber}</span>
                    </div>
                    <div className="flex items-center gap-2">
                      <FaCalendar className="text-primary-200" />
                      <span>Member since {new Date(user?.createdAt).toLocaleDateString()}</span>
                    </div>
                  </div>
                </div>
              </div>
              <button
                onClick={handleLogout}
                className="px-6 py-3 bg-white text-primary-600 rounded-full hover:bg-primary-50 transition-all duration-300 flex items-center gap-2 font-semibold shadow-lg hover:shadow-xl transform hover:-translate-y-1"
              >
                <FaSignOutAlt />
                Logout
              </button>
            </div>
          </div>
        </div>
      </div>

      {/* Main Content */}
      <div className="container mx-auto px-4 py-8 max-w-7xl">
        {/* Tabs */}
        <div className="bg-white rounded-xl shadow-sm mb-8">
          <div className="flex border-b">
            <button
              onClick={() => setActiveTab('favorites')}
              className={`flex-1 px-6 py-4 flex items-center justify-center gap-2 text-lg font-medium transition-all duration-200 ${
                activeTab === 'favorites'
                  ? 'text-primary-600 border-b-2 border-primary-600 bg-primary-50/50'
                  : 'text-gray-600 hover:text-primary-600 hover:bg-gray-50'
              }`}
            >
              <FaHeart className={activeTab === 'favorites' ? 'text-red-500' : 'text-gray-400'} />
              Favorites
            </button>
            <button
              onClick={() => setActiveTab('listings')}
              className={`flex-1 px-6 py-4 flex items-center justify-center gap-2 text-lg font-medium transition-all duration-200 ${
                activeTab === 'listings'
                  ? 'text-primary-600 border-b-2 border-primary-600 bg-primary-50/50'
                  : 'text-gray-600 hover:text-primary-600 hover:bg-gray-50'
              }`}
            >
              <FaList className={activeTab === 'listings' ? 'text-primary-600' : 'text-gray-400'} />
              My Listings
            </button>
          </div>
        </div>

        {/* Tab Content */}
        <div className="bg-white rounded-xl shadow-sm p-6">
          {activeTab === 'favorites' ? (
            <FavoriteProperties />
          ) : (
            <div>
              {properties.length === 0 ? (
                <div className="text-center py-16">
                  <div className="w-24 h-24 bg-gray-100 rounded-full flex items-center justify-center mx-auto mb-6">
                    <FaHome className="text-4xl text-gray-400" />
                  </div>
                  <h3 className="text-2xl font-semibold text-gray-900 mb-3">
                    No Properties Listed Yet
                  </h3>
                  <p className="text-gray-600 mb-8 max-w-md mx-auto">
                    Ready to start your real estate journey? List your first property and reach potential buyers or tenants.
                  </p>
                  <button
                    onClick={() => navigate('/list-property')}
                    className="px-6 py-3 bg-primary-600 text-white rounded-full hover:bg-primary-700 transition-all duration-300 flex items-center gap-2 font-semibold mx-auto"
                  >
                    <FaHome />
                    Add Your First Property
                  </button>
                </div>
              ) : (
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                  {properties.map((property) => (
                    <div
                      key={property.Id}
                      className="group bg-white rounded-xl overflow-hidden border border-gray-200 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1 cursor-pointer"
                      onClick={() => navigate(`/property/${property.Id}`)}
                    >
                      <div className="relative">
                        <img
                          src={property.imageUrl || '/placeholder-property.jpg'}
                          alt={property.title}
                          className="w-full h-48 object-cover"
                        />
                        <div className="absolute top-4 right-4">
                          <span className={`px-3 py-1 rounded-full text-sm font-medium ${
                            property.status === 'ForSale' 
                              ? 'bg-green-100 text-green-800'
                              : 'bg-blue-100 text-blue-800'
                          }`}>
                            {property.status === 'ForSale' ? 'For Sale' : 'For Rent'}
                          </span>
                        </div>
                      </div>
                      <div className="p-5">
                        <h3 className="text-lg font-semibold text-gray-900 mb-2 group-hover:text-primary-600">
                          {property.title}
                        </h3>
                        <div className="flex items-center text-gray-600 mb-4">
                          <FaHome className="mr-2 text-primary-500" />
                          <span>{property.location}</span>
                        </div>
                        <div className="flex justify-between items-center">
                          <span className="text-xl font-bold text-primary-600">
                            ${property.price.toLocaleString()}
                          </span>
                          <span className="text-sm font-medium text-gray-500 bg-gray-100 px-3 py-1 rounded-full">
                            {property.propertyType}
                          </span>
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};
