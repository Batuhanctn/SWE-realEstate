import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { FaUpload, FaHome } from 'react-icons/fa';
import { createProperty } from '../services/propertyService';
import { toast } from 'react-toastify';

/**
 * ListProperty component for creating new property listings
 * Provides a form interface for users to input property details and upload images
 * @returns {JSX.Element} The rendered ListProperty component
 */
export const ListProperty = () => {
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    title: '',
    description: '',
    price: '',
    location: '',
    type: 'apartment',
    status: 'for-sale',
    bedrooms: '',
    bathrooms: '',
    size: '',
    features: [] as string[],
    images: [] as File[],
  });

  const [previewImages, setPreviewImages] = useState<string[]>([]);
  const [isSubmitting, setIsSubmitting] = useState(false);

  /**
   * Handles changes in form input fields
   * @param {React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>} e - The change event
   */
  const handleInputChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>
  ) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  /**
   * Handles image file uploads
   * @param {React.ChangeEvent<HTMLInputElement>} e - The file input change event
   */
  const handleImageUpload = (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = Array.from(e.target.files || []);
    setFormData((prev) => ({
      ...prev,
      images: [...prev.images, ...files],
    }));

    // Create preview URLs
    const newPreviewUrls = files.map((file) => URL.createObjectURL(file));
    setPreviewImages((prev) => [...prev, ...newPreviewUrls]);
  };

  /**
   * Toggles property features selection
   * @param {string} feature - The feature to toggle
   */
  const handleFeatureToggle = (feature: string) => {
    setFormData((prev) => ({
      ...prev,
      features: prev.features.includes(feature)
        ? prev.features.filter((f) => f !== feature)
        : [...prev.features, feature],
    }));
  };

  /**
   * Handles form submission to create a new property listing
   * Validates form data and uploads images
   * @param {React.FormEvent} e - The form submission event
   */
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    // Prevent double submission
    if (isSubmitting) {
      return;
    }
    
    setIsSubmitting(true);
    
    try {
      // Convert form data to match API expectations
      const propertyData = {
        title: formData.title.trim(),
        description: formData.description.trim(),
        price: Number(formData.price),
        location: formData.location.trim(),
        address: formData.location.trim(), // Using location as address for now
        city: 'Default City', // Required by backend
        state: 'Default State', // Required by backend
        zipCode: '00000', // Required by backend
        type: formData.type.charAt(0).toUpperCase() + formData.type.slice(1),
        propertyType: formData.type.charAt(0).toUpperCase() + formData.type.slice(1),
        status: formData.status === 'for-sale' ? 'ForSale' : 'ForRent',
        isForRent: formData.status === 'for-rent', // Required by backend
        bedrooms: Number(formData.bedrooms),
        bathrooms: Number(formData.bathrooms),
        squareMeters: Number(formData.size),
        size: Number(formData.size), // Required by backend as double
        features: formData.features,
        imageUrl: '', // Optional
        imageUrls: [], // Will be populated after image upload
        images: formData.images // For upload
      };

      // Validate required fields
      if (!propertyData.title || !propertyData.description || !propertyData.location) {
        toast.error('Please fill in all required fields');
        return;
      }

      if (propertyData.price <= 0 || propertyData.squareMeters <= 0) {
        toast.error('Price and size must be greater than 0');
        return;
      }
      
      console.log('Submitting property data:', propertyData);
      const result = await createProperty(propertyData);
      console.log('Created property:', result);
      toast.success('Property listed successfully!');
      navigate('/');
    } catch (error: any) {
      const errorMessage = error.response?.data?.message || 'Failed to list property. Please try again.';
      toast.error(errorMessage);
      console.error('Error listing property:', error);
      if (error.response?.data) {
        console.error('Server error details:', error.response.data);
      }
    } finally {
      setIsSubmitting(false);
    }
  };

  const features = [
    'Air Conditioning',
    'Heating',
    'Parking',
    'Pool',
    'Garden',
    'Security System',
    'Internet',
    'Cable TV',
  ];

  return (
    <div className="bg-gray-50 min-h-screen py-12 pt-28">
      <div className="container mx-auto px-4 max-w-3xl">
        <div className="bg-white rounded-2xl shadow-xl p-8">
          <div className="flex items-center gap-4 mb-8">
            <FaHome className="text-4xl text-primary-500" />
            <div>
              <h1 className="text-2xl font-bold">List Your Property</h1>
              <p className="text-gray-400">Fill in the details below to list your property</p>
            </div>
          </div>

          <form onSubmit={handleSubmit} className="space-y-8">
            {/* Basic Information */}
            <div className="space-y-4">
              <h2 className="text-xl font-semibold">Basic Information</h2>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-300 mb-1">
                    Title
                  </label>
                  <input
                    type="text"
                    name="title"
                    value={formData.title}
                    onChange={handleInputChange}
                    placeholder="e.g., Modern Apartment in City Center"
                    className="w-full px-4 py-2 bg-gray-100 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent text-gray-600 placeholder-gray-400"
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-300 mb-1">
                    Price
                  </label>
                  <input
                    type="number"
                    name="price"
                    value={formData.price}
                    onChange={handleInputChange}
                    placeholder="e.g., 500000"
                    className="w-full px-4 py-2 bg-gray-100 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent text-gray-600 placeholder-gray-400"
                    required
                  />
                </div>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-300 mb-1">
                  Description
                </label>
                <textarea
                  name="description"
                  value={formData.description}
                  onChange={handleInputChange}
                  placeholder="Describe your property in detail..."
                  rows={4}
                  className="w-full px-4 py-2 bg-gray-100 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent text-gray-600 placeholder-gray-400"
                  required
                />
              </div>
            </div>

            {/* Property Details */}
            <div className="space-y-4">
              <h2 className="text-xl font-semibold">Property Details</h2>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-300 mb-1">
                    Type
                  </label>
                  <select
                    name="type"
                    value={formData.type}
                    onChange={handleInputChange}
                    className="w-full px-4 py-2 bg-gray-100 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent text-gray-600"
                  >
                    <option value="apartment">Apartment</option>
                    <option value="house">House</option>
                    <option value="villa">Villa</option>
                  </select>
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-300 mb-1">
                    Status
                  </label>
                  <select
                    name="status"
                    value={formData.status}
                    onChange={handleInputChange}
                    className="w-full px-4 py-2 bg-gray-100 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent text-gray-600"
                  >
                    <option value="for-sale">For Sale</option>
                    <option value="for-rent">For Rent</option>
                  </select>
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-300 mb-1">
                    Location
                  </label>
                  <input
                    type="text"
                    name="location"
                    value={formData.location}
                    onChange={handleInputChange}
                    placeholder="e.g., Downtown, New York"
                    className="w-full px-4 py-2 bg-gray-100 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent text-gray-600 placeholder-gray-400"
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-300 mb-1">
                    Size (m²)
                  </label>
                  <input
                    type="number"
                    name="size"
                    value={formData.size}
                    onChange={handleInputChange}
                    placeholder="e.g., 120"
                    className="w-full px-4 py-2 bg-gray-100 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent text-gray-600 placeholder-gray-400"
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-300 mb-1">
                    Bedrooms
                  </label>
                  <input
                    type="number"
                    name="bedrooms"
                    value={formData.bedrooms}
                    onChange={handleInputChange}
                    placeholder="e.g., 3"
                    className="w-full px-4 py-2 bg-gray-100 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent text-gray-600 placeholder-gray-400"
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-300 mb-1">
                    Bathrooms
                  </label>
                  <input
                    type="number"
                    name="bathrooms"
                    value={formData.bathrooms}
                    onChange={handleInputChange}
                    placeholder="e.g., 2"
                    className="w-full px-4 py-2 bg-gray-100 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent text-gray-600 placeholder-gray-400"
                    required
                  />
                </div>
              </div>
            </div>

            {/* Features */}
            <div className="space-y-4">
              <h2 className="text-xl font-semibold">Features</h2>
              <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                {features.map((feature) => (
                  <label
                    key={feature}
                    className="flex items-center space-x-2 cursor-pointer"
                  >
                    <input
                      type="checkbox"
                      checked={formData.features.includes(feature)}
                      onChange={() => handleFeatureToggle(feature)}
                      className="w-4 h-4 text-primary-600 bg-gray-100 border-gray-300 rounded focus:ring-primary-500"
                    />
                    <span className="text-sm text-gray-600">{feature}</span>
                  </label>
                ))}
              </div>
            </div>

            {/* Image Upload */}
            <div className="space-y-4">
              <h2 className="text-xl font-semibold">Property Images</h2>
              <div className="border-2 border-dashed border-gray-300 rounded-lg p-8">
                <div className="text-center">
                  <FaUpload className="mx-auto text-4xl text-gray-400 mb-4" />
                  <div className="space-y-2">
                    <label
                      htmlFor="images"
                      className="px-4 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700 transition-colors cursor-pointer inline-block"
                    >
                      Choose Files
                    </label>
                    <input
                      id="images"
                      type="file"
                      multiple
                      accept="image/*"
                      onChange={handleImageUpload}
                      className="hidden"
                    />
                    <p className="text-sm text-gray-400">
                      Upload up to 10 images (PNG, JPG)
                    </p>
                  </div>
                </div>
                {previewImages.length > 0 && (
                  <div className="mt-8 grid grid-cols-2 md:grid-cols-4 gap-4">
                    {previewImages.map((url, index) => (
                      <div
                        key={index}
                        className="relative aspect-square rounded-lg overflow-hidden"
                      >
                        <img
                          src={url}
                          alt={`Preview ${index + 1}`}
                          className="w-full h-full object-cover"
                        />
                        <button
                          type="button"
                          onClick={() => {
                            setPreviewImages((prev) =>
                              prev.filter((_, i) => i !== index)
                            );
                            setFormData((prev) => ({
                              ...prev,
                              images: prev.images.filter((_, i) => i !== index),
                            }));
                          }}
                          className="absolute top-2 right-2 p-1 bg-red-500 text-white rounded-full hover:bg-red-600 transition-colors"
                        >
                          ×
                        </button>
                      </div>
                    ))}
                  </div>
                )}
              </div>
            </div>

            {/* Submit Button */}
            <div className="flex justify-end">
              <button
                type="submit"
                disabled={isSubmitting}
                className={`px-8 py-3 bg-primary-500 text-white rounded-lg hover:bg-primary-600 transition-colors
                  ${isSubmitting ? 'opacity-50 cursor-not-allowed' : ''}`}
              >
                {isSubmitting ? 'Listing Property...' : 'List Property'}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};
